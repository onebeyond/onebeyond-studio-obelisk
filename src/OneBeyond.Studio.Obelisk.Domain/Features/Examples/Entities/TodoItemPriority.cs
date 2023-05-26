using Ardalis.SmartEnum;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Examples;

public sealed class TodoItemPriority : SmartEnum<TodoItemPriority, int>
{
    public static readonly TodoItemPriority Normal = new("Normal", 0);
    public static readonly TodoItemPriority High = new("High", 1);
    public static readonly TodoItemPriority Critical = new("Critical", 2);

    private TodoItemPriority(
        string name,
        int value)
        : base(name, value)
    {
    }
}
