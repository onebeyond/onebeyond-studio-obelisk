using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace OneBeyond.Studio.Obelisk.WebApi.OpenApi;

/// <summary>
/// Sets the OpenAPI document <see cref="OpenApiInfo"/> based on the API version
/// description matching the document name, including deprecation notices.
/// </summary>
internal sealed class VersionInfoDocumentTransformer(
    IApiVersionDescriptionProvider versionProvider) : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var description = versionProvider.ApiVersionDescriptions
            .FirstOrDefault(d => d.GroupName == context.DocumentName);

        document.Info = new()
        {
            Title = ApiConstants.ApiTitle,
            Version = description is not null
                ? $"v{description.ApiVersion}"
                : context.DocumentName,
            Description = description?.IsDeprecated is true
                ? "This API version has been deprecated."
                : null,
        };

        return Task.CompletedTask;
    }
}
