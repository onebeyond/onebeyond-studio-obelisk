using System;
using EnsureThat;

namespace OneBeyond.Studio.Obelisk.Domain.Attributes;

//TODO I DO NOT LIKE IT IS IN DOMAIN :(

[AttributeUsage(AttributeTargets.Property)]
public sealed class BulkUpdateExcludeAttribute : Attribute
{
}
