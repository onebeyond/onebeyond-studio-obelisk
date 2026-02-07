using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using OneBeyond.Studio.Hosting.AspNet.ModelBinders.MixedSource;

namespace OneBeyond.Studio.Obelisk.WebApi.OpenApi;

/// <summary>
/// When the <see cref="FromMixedSourceAttribute"/> attribute is used on an action parameter,
/// the default API explorer incorrectly infers it as a query parameter. This transformer
/// replaces that with a proper JSON request body whose schema is derived from the type's
/// constructor parameters that are not already bound from the route.
/// </summary>
internal sealed class FromMixedSourceOperationTransformer : IOpenApiOperationTransformer
{
    public async Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (context.Description.ActionDescriptor is not ControllerActionDescriptor descriptor)
        {
            return;
        }

        var parameters = descriptor.MethodInfo.GetParameters();

        foreach (var parameter in parameters)
        {
            var attribute = parameter.GetCustomAttributes(inherit: true)
                .OfType<FromMixedSourceAttribute>()
                .FirstOrDefault();

            if (attribute is null)
            {
                continue;
            }

            // Get the constructor with the most parameters — this matches the MixedSource
            // binder's own resolution strategy.
            var ctor = parameter.ParameterType
                .GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault();

            if (ctor is null)
            {
                continue;
            }

            var schemaProperties = new Dictionary<string, IOpenApiSchema>();

            foreach (var ctorParam in ctor.GetParameters())
            {
                if (ctorParam.Name is null)
                {
                    continue;
                }

                // Skip parameters that are already bound from the route (they appear as
                // existing operation parameters). Use case-insensitive comparison because
                // ASP.NET model binding is case-insensitive.
                if (operation.Parameters?.Any(p =>
                    string.Equals(p.Name, ctorParam.Name, StringComparison.OrdinalIgnoreCase)) is true)
                {
                    continue;
                }

                var schema = await context.GetOrCreateSchemaAsync(
                    ctorParam.ParameterType,
                    parameterDescription: null,
                    cancellationToken);

                schemaProperties[ctorParam.Name] = schema;
            }

            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    [MediaTypeNames.Application.Json] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = JsonSchemaType.Object,
                            Properties = schemaProperties,
                        },
                    },
                },
            };

            // Remove the incorrectly-inferred query parameter for the [FromMixedSource] object.
            var wrongParam = operation.Parameters?.FirstOrDefault(p =>
                string.Equals(p.Name, parameter.Name, StringComparison.OrdinalIgnoreCase));

            if (wrongParam is not null)
            {
                operation.Parameters!.Remove(wrongParam);
            }

            // Only process the first [FromMixedSource] parameter (matches old filter behaviour).
            break;
        }
    }
}
