using Autofac;
using EnsureThat;

namespace OneBeyond.Studio.Obelisk.Authentication.Application.DependencyInjection;

public static class ContainerBuilderExtensions
{
    public static ContainerBuilder AddAuthApplication(this ContainerBuilder containerBuilder)
    {
        EnsureArg.IsNotNull(containerBuilder, nameof(containerBuilder));

        //It does not do anything at the moment, but if you need any additional IoC setup for Authentication applicaiton, please do it here

        return containerBuilder;
    }
}
