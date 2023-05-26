using System;
using EnsureThat;

namespace OneBeyond.Studio.Obelisk.Domain.Attributes;

//TODO I DO NOT LIKE IT IS IN DOMAIN :(

[AttributeUsage(AttributeTargets.Property)]
public sealed class BulkUpdateColumnNameAttribute : Attribute
{
    public BulkUpdateColumnNameAttribute(string name)
    {
        Name = EnsureArg.IsNotNullOrWhiteSpace(name, nameof(name));
    }

    public string Name { get; }
}
