using System;
using System.Linq;
using System.Reflection;
using Ardalis.SmartEnum;
using EnsureThat;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OneBeyond.Studio.Obelisk.WebApi.Swagger;

internal sealed class SmartEnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        EnsureArg.IsNotNull(schema, nameof(schema));
        EnsureArg.IsNotNull(context, nameof(context));

        var type = context.Type;

        if (!type.IsSmartEnum())
        {
            return;
        }

        if (type.TryGetValues(out var enumValues))
        {
            var openApiValues = new OpenApiArray();
            openApiValues.AddRange(enumValues.Select(d => new OpenApiString((string)d)));

            schema.Type = "string";
            schema.Enum = openApiValues;
            schema.Properties = null;
        }
    }
}
