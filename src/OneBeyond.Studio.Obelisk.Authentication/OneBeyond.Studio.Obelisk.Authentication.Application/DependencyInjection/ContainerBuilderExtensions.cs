using System.Reflection;
using Autofac;
using EnsureThat;
using OneBeyond.Studio.Application.SharedKernel.Authorization;
using OneBeyond.Studio.Application.SharedKernel.DependencyInjection;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.DependencyInjection;

public static class ContainerBuilderExtensions
{
    public static ContainerBuilder AddAuthApplication(this ContainerBuilder containerBuilder)
    {
        EnsureArg.IsNotNull(containerBuilder, nameof(containerBuilder));

        //It does not do anything at the moment, but if you need any additional IoC setup for Authentication application, please do it here
        var thisAssembly = Assembly.GetExecutingAssembly();
        containerBuilder.AddMediatorRequestHandlers(thisAssembly);
        containerBuilder.AddAuthorizationRequirementHandlers(new AuthorizationOptions
        {
            // Set to false to force ALL commands to have authorization on them
            AllowUnattributedRequests = true,
        });


        return containerBuilder;
    }
}
