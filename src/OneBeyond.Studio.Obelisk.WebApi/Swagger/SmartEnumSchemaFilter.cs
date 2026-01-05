using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Ardalis.SmartEnum;
using EnsureThat;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OneBeyond.Studio.Obelisk.WebApi.Swagger;

internal sealed class SmartEnumSchemaFilter : ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        EnsureArg.IsNotNull(schema, nameof(schema));
        EnsureArg.IsNotNull(context, nameof(context));

        if (schema is not OpenApiSchema concrete)
        {
            return;
        }

        var type = context.Type;

        if (!type.IsSmartEnum())
        {
            return;
        }

        if (type.TryGetValues(out var enumValues))
        {
            var openApiValues = new List<JsonNode>();

            openApiValues.AddRange(enumValues.Select(d => new JsonObject
            {
                ["value"] = d.ToString()
            }));            

            concrete.Type = JsonSchemaType.String;
            concrete.Enum = openApiValues;
            concrete.Properties = null;
        }
    }
}
