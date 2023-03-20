using System;
using EnsureThat;
using OneBeyond.Studio.Domain.SharedKernel.Entities;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;

public sealed class UserLoginLog : AggregateRoot<int>
{
    public UserLoginLog(string username, bool success, string? status)
    {
        EnsureArg.IsNotNullOrWhiteSpace(username, nameof(username));

        Username = username;
        Success = success;
        Status = status;
        AttemptDate = DateTimeOffset.UtcNow;
    }

#nullable disable
    private UserLoginLog()
    {
    }
#nullable restore

    public string Username { get; private set; }
    public bool Success { get; private set; }
    public string? Status { get; private set; }
    public DateTimeOffset AttemptDate { get; private set; }
}
