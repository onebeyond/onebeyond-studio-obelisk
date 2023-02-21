using System;
using System.Transactions;
using EnsureThat;
using Microsoft.Extensions.DependencyInjection;
using OneBeyond.Studio.Application.SharedKernel.DataAccessPolicies;
using OneBeyond.Studio.Application.SharedKernel.Repositories;
using OneBeyond.Studio.DataAccess.EFCore.DependencyInjection;
using OneBeyond.Studio.Obelisk.Infrastructure.Data;

namespace OneBeyond.Studio.Obelisk.Infrastructure.DependencyInjection;

internal sealed class DataAccessBuilder : IDataAccessBuilder
{
    private readonly IServiceCollection _services;
    private readonly OneBeyond.Studio.DataAccess.EFCore.DependencyInjection.IDataAccessBuilder _dataAccessBuilder;

    public DataAccessBuilder(
        IServiceCollection services,
        OneBeyond.Studio.DataAccess.EFCore.DependencyInjection.IDataAccessBuilder dataAccessBuilder)
    {
        EnsureArg.IsNotNull(services, nameof(services));
        EnsureArg.IsNotNull(dataAccessBuilder, nameof(dataAccessBuilder));

        _services = services;
        _dataAccessBuilder = dataAccessBuilder;

        services.AddScoped(typeof(IRORepository<>), typeof(RORepository<>));
        services.AddScoped(typeof(IRORepository<,>), typeof(RORepository<,>));
        services.AddScoped(typeof(IRWRepository<,>), typeof(RWRepository<,>));
        services.AddSingleton(typeof(IRODataAccessPolicyProvider<>), typeof(AllowDataAccessPolicyProvider<>));
        services.AddSingleton(typeof(IRWDataAccessPolicyProvider<>), typeof(AllowDataAccessPolicyProvider<>));
        services.AddScoped(typeof(IAggregateRootRWRepository<,,>), typeof(AggregateRootRWRepository<,,>));
    }

    public IDataAccessBuilder WithUnitOfWork(TimeSpan? timeout = default, IsolationLevel? isolationLevel = default)
    {
        _dataAccessBuilder.WithUnitOfWork(timeout, isolationLevel);
        return this;
    }

    public IDataAccessBuilder WithDomainEvents(bool isReceiverHost = false)
    {
        _dataAccessBuilder.WithDomainEvents();
        if (isReceiverHost)
        {
            _services.AddMsSQLRaisedDomainEventReceiver<DomainContext>();
        }
        return this;
    }
}
