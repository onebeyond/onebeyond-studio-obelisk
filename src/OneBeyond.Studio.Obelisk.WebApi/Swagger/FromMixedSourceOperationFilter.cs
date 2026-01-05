using HandlebarsDotNet;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi;
using OneBeyond.Studio.Hosting.AspNet.ModelBinders.MixedSource;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OneBeyond.Studio.Obelisk.WebApi.Swagger;

/// <summary>
/// This operation filter is required to make sure that when the [FromMixedSource]
/// attribute is used, it gets translated in Swagger as a Body request and not as a Query string
/// </summary>
internal sealed class FromMixedSourceOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription.ActionDescriptor is not ControllerActionDescriptor controllerActionDescriptor)
        {
            return;
        }

        var parameters = controllerActionDescriptor.MethodInfo.GetParameters();

        foreach (var parameter in parameters)
        {
            if (ProcessContollerActionParameter(operation, context, parameter))
            {
                break;
            }
        }
    }

    private static bool ProcessContollerActionParameter(
        OpenApiOperation operation, 
        OperationFilterContext context, 
        ParameterInfo parameter)
    {
        var customFromBodyAttribute = parameter.GetCustomAttributes(true).OfType<FromMixedSourceAttribute>().FirstOrDefault();

        if (customFromBodyAttribute is null)
        {
            return false;
        }

        // Get the constructor with the highest number of parameters
        var ctor = parameter.ParameterType.GetConstructors().OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();

        if (ctor is null)
        {
            return false;
        }

        var properties = ctor.GetParameters().ToDictionary(p => p.Name!, p => p.ParameterType);

        var schema = new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Properties = GetSchemaProperties(operation, context, properties)
        };

        operation.RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType>
                {
                    {
                        "application/json", new OpenApiMediaType
                        {
                            Schema = schema
                        }
                    }
                }
        };

        // Get rid of wrong Query parameter for the [FromMixedSource] object
        if (operation.Parameters!.FirstOrDefault(p => p.Name == parameter.Name) != null)
        {
            operation.Parameters!.Remove(operation.Parameters!.First(p => p.Name == parameter.Name));
        }
        

        return true;
    }

    private static Dictionary<string, IOpenApiSchema> GetSchemaProperties(
        OpenApiOperation operation,
        OperationFilterContext context,
        Dictionary<string, System.Type> properties)
    {
        var schemaProperties = new Dictionary<string, IOpenApiSchema>();
        foreach (var property in properties)
        {
            if (!operation.Parameters!.Any(p => p.Name == property.Key)) // Avoid adding the property if it's bound with a Query parameter
            {
                var schemaRepository = context.SchemaRepository ?? new SchemaRepository();
                var generatedSchema = context.SchemaGenerator.GenerateSchema(property.Value, schemaRepository);

                schemaProperties[property.Key!] = generatedSchema;
            }
        }
        return schemaProperties;
    }
}
