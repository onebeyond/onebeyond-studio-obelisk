using System.Text.Json.Serialization;
using Ardalis.SmartEnum;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Examples;

public sealed class TodoItemPriority : SmartEnum<TodoItemPriority, int>
{
    public static readonly TodoItemPriority Normal = new("Normal", 0);
    public static readonly TodoItemPriority High = new("High", 1);
    public static readonly TodoItemPriority Critical = new("Critical", 2);

    //TODO ASK FABIO NOTE IT IS PUBLIC AS WE NEED TO DESERIALIZE IT
    public TodoItemPriority(
        string name,
        int value)
        : base(name, value)
    {
    }
}
