using System;
using System.Transactions;

namespace OneBeyond.Studio.Obelisk.Infrastructure.DependencyInjection;

public interface IDataAccessBuilder
{
    IDataAccessBuilder WithUnitOfWork(TimeSpan? timeout = default, IsolationLevel? isolationLevel = default);

    IDataAccessBuilder WithDomainEvents(bool isReceiverHost = false);

    IDataAccessBuilder WithChangeTracking();

}
