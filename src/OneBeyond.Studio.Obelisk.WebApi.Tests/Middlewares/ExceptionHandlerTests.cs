using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Microsoft.AspNetCore.Mvc;

namespace OneBeyond.Studio.Obelisk.WebApi.Tests.Middlewares;

public class ExceptionHandlerTests : IClassFixture<TestServerFixture>
{
    private const string ProblemContent = "application/problem+json";
    private const string TraceIdKey = "traceId";

    private readonly HttpClient _client;

    public ExceptionHandlerTests(TestServerFixture testServer)
    {
        _client = testServer.Client;
    }

    [Fact]
    public async Task NoException_Succeeds()
    {
        // Act
        var response = await _client.GetAsync("/health/live");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task ValidationError_ReturnsBadRequest()
    {
        // Arrange — an empty body triggers ASP.NET model validation, not
        // a ValidationException. The [ApiController] attribute handles this
        // automatically and returns ProblemDetails with a 400 status.
        var invalidRequest = new { };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/v1", invalidRequest);

        // Assert
        using var _ = new AssertionScope();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails?.Title.Should().Be("One or more validation errors occurred.");

        var traceId = problemDetails?.Extensions[TraceIdKey]?.ToString();
        traceId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task EntityNotFoundException_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/users/v1/{nonExistentId}");

        // Assert
        using var _ = new AssertionScope();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var mediaType = response.Content.Headers.ContentType?.MediaType;
        mediaType.Should().Be(ProblemContent);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails?.Title.Should().Be("Not Found");

        var traceId = problemDetails?.Extensions[TraceIdKey]?.ToString();
        traceId.Should().NotBeNullOrEmpty();
    }
}
