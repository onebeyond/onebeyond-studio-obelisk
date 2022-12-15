using EnsureThat;
using OneBeyond.Studio.Domain.SharedKernel.Entities;

namespace OneBeyond.Studio.Obelisk.Domain.Features.EmailTemplates.Entities;

public sealed class EmailTemplate : DomainEntity<string>, IAggregateRoot
{
    public EmailTemplate(
        string id, //Note! For EmailTemplate Id is the Tag!
        string subject,
        string body,
        string? description)
        : base(id)
    {
        EnsureArg.IsNotNullOrWhiteSpace(subject, nameof(subject));
        EnsureArg.IsNotNullOrWhiteSpace(body, nameof(body));

        Subject = subject;
        Body = body;
        Description = description;
    }

    public string Subject { get; private set; }
    public string Body { get; private set; }
    public string? Description { get; private set; }
}
