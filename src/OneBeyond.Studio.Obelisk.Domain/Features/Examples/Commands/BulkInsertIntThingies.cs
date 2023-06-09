using MediatR;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Examples.Commands;

public sealed record BulkInsertIntThingies(int Count): IRequest
{
}
