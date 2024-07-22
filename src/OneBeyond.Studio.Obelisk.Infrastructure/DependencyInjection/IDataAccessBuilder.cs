using System;
using System.Transactions;

namespace OneBeyond.Studio.Obelisk.Infrastructure.DependencyInjection;

public interface IDataAccessBuilder
{
    /// <summary>
    /// Adds transaction scope to all requests. Note, in order to enable this, cloudDeployment must be disabled in AddDataAccess, as retries-on-failure is mutually incompatible with transaction scope.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="isolationLevel"></param>
    /// <returns></returns>
    IDataAccessBuilder WithUnitOfWork(TimeSpan? timeout = default, IsolationLevel? isolationLevel = default);

    IDataAccessBuilder WithDomainEvents(bool isReceiverHost = false);
}
