using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using OneBeyond.Studio.Obelisk.WebApi.Middlewares;

namespace OneBeyond.Studio.Obelisk.WebApi.Tests.Middlewares;

public class ErrorResultGeneratorMiddlewareTests : IClassFixture<TestServerFixture>
{
    private readonly HttpClient _client;

    public ErrorResultGeneratorMiddlewareTests(TestServerFixture testServer)
    {
        _client = testServer.Client;
    }

    [Fact]
    public async Task InvokeAsync_NoException_Succeeds()
    {
        // Act
        var response = await _client.GetAsync("/health/live");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_ValidationException_ReturnsBadRequest()
    {
        // Arrange
        var invalidRequest = new { };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/v1", invalidRequest);

        // Assert
        using var _ = new AssertionScope();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails?.Title.Should().Be("One or more validation errors occurred.");

        var traceId = problemDetails?.Extensions[ErrorResultGeneratorMiddleware.TraceIdKey]?.ToString();
        traceId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task InvokeAsync_EntityNotFoundException_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/users/v1/{nonExistentId}");

        // Assert
        using var _ = new AssertionScope();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var mediaType = response.Content.Headers.ContentType?.MediaType;
        mediaType.Should().Be(ErrorResultGeneratorMiddleware.ProblemContent);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails?.Title.Should().Be("Not Found");

        var traceId = problemDetails?.Extensions[ErrorResultGeneratorMiddleware.TraceIdKey]?.ToString();
        traceId.Should().NotBeNullOrEmpty();
    }
}
