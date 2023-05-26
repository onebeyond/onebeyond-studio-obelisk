using System;
using EnsureThat;

namespace OneBeyond.Studio.Obelisk.Domain.Attributes;

//TODO I DO NOT LIKE IT IS IN DOMAIN :(

[AttributeUsage(AttributeTargets.Class)]
public sealed class BulkUpdateTableNameAttribute : Attribute
{
    public BulkUpdateTableNameAttribute(string name)
    {
        Name = EnsureArg.IsNotNullOrWhiteSpace(name, nameof(name));
    }

    public string Name { get; }
}
