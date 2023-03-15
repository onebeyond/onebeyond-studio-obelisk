using System;
using System.IO;
using System.Reflection;
using EnsureThat;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OneBeyond.Studio.Obelisk.WebApi.Swagger;

internal sealed class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        EnsureArg.IsNotNull(provider, nameof(provider));

        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        EnsureArg.IsNotNull(options, nameof(options));

        options.CustomSchemaIds((type) => type.FullName);

        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);
        options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
        options.OperationFilter<FromMixedSourceOperationFilter>();
    }

    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var info = new OpenApiInfo()
        {
            Title = SwaggerConstants.APITitle,
            Version = $"v{description.ApiVersion}",
        };

        if (description.IsDeprecated)
        {
            info.Description += " This API version has been deprecated.";
        }

        return info;
    }
}
