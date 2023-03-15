using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using OneBeyond.Studio.Hosting.AspNet.ModelBinders.MixedSource;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace OneBeyond.Studio.Obelisk.WebApi.Swagger;

/// <summary>
/// This operation filter is required to make sure that when the [FromMixedSource]
/// attribute is used, it gets translated in Swagger as a Body request and not as a Query string
/// </summary>
internal sealed class FromMixedSourceOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
        {
            var parameters = controllerActionDescriptor.MethodInfo.GetParameters();
            foreach (var parameter in parameters)
            {
                var customFromBodyAttribute = parameter.GetCustomAttributes(true).OfType<FromMixedSourceAttribute>().FirstOrDefault();
                if (customFromBodyAttribute != null)
                {
                    // Get the constructor with the highest number of parameters
                    var ctor = parameter.ParameterType.GetConstructors().OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();
                    if (ctor != null)
                    {
                        var properties = ctor.GetParameters().ToDictionary(p => p.Name!, p => p.ParameterType);
                        var schemaProperties = new Dictionary<string, OpenApiSchema>();
                        foreach (var property in properties)
                        {
                            if (!operation.Parameters.Any(p => p.Name == property.Key)) // Avoid adding the property if it's bound with a Query parameter
                            {
                                var schemaRepository = context.SchemaRepository ?? new SchemaRepository();
                                var generatedSchema = context.SchemaGenerator.GenerateSchema(property.Value, schemaRepository);

                                schemaProperties[property.Key!] = generatedSchema;
                            }
                        }

                        var schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = schemaProperties
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
                        operation.Parameters.Remove(operation.Parameters.FirstOrDefault(p => p.Name == parameter.Name));
                    }

                    break;
                }
            }
        }
    }
}
