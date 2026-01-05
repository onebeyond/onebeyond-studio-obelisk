using System.Reflection;
using Autofac;
using EnsureThat;
using OneBeyond.Studio.Application.SharedKernel.Authorization;
using OneBeyond.Studio.Application.SharedKernel.DependencyInjection;
using OneBeyond.Studio.Application.SharedKernel.DomainEvents;
using OneBeyond.Studio.Core.Mediator.Notifications;
using OneBeyond.Studio.Obelisk.Application.Services.AuthenticationFlows;
using OneBeyond.Studio.Obelisk.Application.Services.EmailTemplateService;
using OneBeyond.Studio.Obelisk.Authentication.Domain.AuthenticationFlows;

namespace OneBeyond.Studio.Obelisk.Application.DependencyInjection;

public static class ContainerBuilderExtensions
{
    public static ContainerBuilder AddApplication(this ContainerBuilder containerBuilder)
    {
        EnsureArg.IsNotNull(containerBuilder, nameof(containerBuilder));

        var thisAssembly = Assembly.GetExecutingAssembly();

        containerBuilder.AddMediatorRequestHandlers(thisAssembly);

        containerBuilder.RegisterAssemblyTypes(thisAssembly)
            .AsClosedTypesOf(typeof(INotificationHandler<>));

        containerBuilder.RegisterType<EmailTemplateService>()
            .As<IEmailTemplateService>()
            .InstancePerDependency();

        containerBuilder.RegisterAssemblyTypes(thisAssembly)
            .AsClosedTypesOf(typeof(IPreSaveDomainEventHandler<>))
            .InstancePerLifetimeScope();

        containerBuilder.RegisterAssemblyTypes(thisAssembly)
            .AsClosedTypesOf(typeof(IPostSaveDomainEventHandler<>))
            .InstancePerLifetimeScope();

        containerBuilder.RegisterType<AuthenticationFlowHandler>()
            .As<IAuthenticationFlowHandler>()
            .InstancePerLifetimeScope();

        return containerBuilder.AddAuthorizationHandlers();
    }

    private static ContainerBuilder AddAuthorizationHandlers(
            this ContainerBuilder containerBuilder)
    {
        containerBuilder.AddAuthorizationRequirementHandlers(
            new AuthorizationOptions
            {
                AllowUnattributedRequests = true
            },
            Assembly.GetExecutingAssembly());

        return containerBuilder;
    }
}
