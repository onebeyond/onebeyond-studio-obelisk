using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.SmartEnum;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace OneBeyond.Studio.Obelisk.WebApi.OpenApi;

/// <summary>
/// Converts SmartEnum types to <c>type: string</c> with enum values in the OpenAPI schema.
/// </summary>
internal sealed class SmartEnumSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        var type = context.JsonTypeInfo.Type;

        if (!type.IsSmartEnum() || !type.TryGetValues(out var enumValues))
        {
            return Task.CompletedTask;
        }

        schema.Type = JsonSchemaType.String;
        schema.Enum = enumValues
            .Select(JsonNode (v) => JsonValue.Create(v.ToString()!))
            .ToList();

        schema.Properties = null;

        return Task.CompletedTask;
    }
}
