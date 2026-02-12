using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using OneBeyond.Studio.Hosting.AspNet.ModelBinders.MixedSource;

namespace OneBeyond.Studio.Obelisk.WebApi.OpenApi;

/// <summary>
/// Corrects the OpenAPI document for parameters decorated with <see cref="FromMixedSourceAttribute"/>.
/// The mixed-source binder assembles a command from multiple sources (route + body), but without
/// this transformer the framework incorrectly represents the parameter as a query string.
/// This transformer builds a proper request body schema from the constructor parameters that are
/// not already bound from the route.
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
        var mixedSourceParam = parameters.FirstOrDefault(
            p => p.GetCustomAttribute<FromMixedSourceAttribute>() is not null);

        if (mixedSourceParam is null)
        {
            return;
        }

        var paramType = mixedSourceParam.ParameterType;

        // Convention: use the constructor with the most parameters (same as the mixed-source binder).
        var ctor = paramType.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            .OrderByDescending(c => c.GetParameters().Length)
            .FirstOrDefault();

        if (ctor is null)
        {
            return;
        }

        // Determine which constructor parameters are already represented as route/path parameters.
        var existingParamNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (operation.Parameters is not null)
        {
            foreach (var p in operation.Parameters)
            {
                if (p.Name is not null)
                {
                    existingParamNames.Add(p.Name);
                }
            }
        }

        // Build property schemas for every constructor parameter NOT already bound from the route.
        var bodyProperties = new Dictionary<string, IOpenApiSchema>();
        var requiredProperties = new HashSet<string>();

        foreach (var ctorParam in ctor.GetParameters())
        {
            if (existingParamNames.Contains(ctorParam.Name!))
            {
                continue;
            }

            var paramDescription = new ApiParameterDescription
            {
                Name = ctorParam.Name!,
                Type = ctorParam.ParameterType,
                Source = BindingSource.Body,
            };

            var schema = await context.GetOrCreateSchemaAsync(
                ctorParam.ParameterType, paramDescription, cancellationToken);

            bodyProperties[ctorParam.Name!] = schema;

            // Treat non-nullable parameters as required.
            if (!IsNullable(ctorParam))
            {
                requiredProperties.Add(ctorParam.Name!);
            }
        }

        if (bodyProperties.Count == 0)
        {
            return;
        }

        // Build the composite request body schema.
        var bodySchema = new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Properties = bodyProperties,
            Required = requiredProperties,
        };

        operation.RequestBody = new OpenApiRequestBody
        {
            Required = true,
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/json"] = new OpenApiMediaType { Schema = bodySchema },
            },
        };

        // Remove the incorrectly-inferred query parameter for the mixed-source parameter.
        if (operation.Parameters is not null)
        {
            var toRemove = operation.Parameters
                .Where(p => string.Equals(p.Name, mixedSourceParam.Name, StringComparison.OrdinalIgnoreCase)
                    && p.In == ParameterLocation.Query)
                .ToList();

            foreach (var p in toRemove)
            {
                operation.Parameters.Remove(p);
            }
        }
    }

    private static bool IsNullable(ParameterInfo parameter)
    {
        if (Nullable.GetUnderlyingType(parameter.ParameterType) is not null)
        {
            return true;
        }

        var context = new NullabilityInfoContext();
        var info = context.Create(parameter);
        return info.WriteState is NullabilityState.Nullable;
    }
}
