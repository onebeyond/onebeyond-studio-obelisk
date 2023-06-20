using OneBeyond.Studio.Domain.SharedKernel.Entities;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Examples.Entities;

public sealed class IntIdThingy : AggregateRoot<int>
{
    public IntIdThingy(
        string name,
        bool isImportant)
    {
        Name = name;
        IsImportant = isImportant;
    }

    public string Name { get; private set; } = default!;

    public bool IsImportant { get; private set; }
}
