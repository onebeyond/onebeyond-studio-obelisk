using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneBeyond.Studio.Obelisk.Authentication.Domain;

public record ChangePasswordResult
{
    public ChangePasswordResult(
        ChangePasswordStatus status)
        : this(status, string.Empty)
    {
    }

    public ChangePasswordResult(
        ChangePasswordStatus status,
        string message)
    {
        Status = status;
        StatusMessage = message;
    }

    public ChangePasswordStatus Status { get; }
    public string StatusMessage { get; }
}

public enum ChangePasswordStatus
{
    Success = 0,
    UnknownUser = 1,
    UserDoesNotHaveAPasswordYet = 2,
    OperationFailure = 3
}
