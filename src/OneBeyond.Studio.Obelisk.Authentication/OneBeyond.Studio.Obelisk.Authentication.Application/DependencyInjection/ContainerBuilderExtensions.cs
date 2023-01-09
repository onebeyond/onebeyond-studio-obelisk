using System.Reflection;
using Autofac;
using EnsureThat;
using OneBeyond.Studio.Application.SharedKernel.DependencyInjection;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.DependencyInjection;

public static class ContainerBuilderExtensions
{
    public static ContainerBuilder AddAuthDomain(this ContainerBuilder containerBuilder)
    {
        EnsureArg.IsNotNull(containerBuilder, nameof(containerBuilder));

        var thisAssembly = Assembly.GetExecutingAssembly();

        containerBuilder.AddMediatorRequestHandlers(thisAssembly);

        return containerBuilder;
    }
}
