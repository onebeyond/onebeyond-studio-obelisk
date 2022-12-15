namespace OneBeyond.Studio.Obelisk.Application.Services.EmailTemplateService;

internal sealed record EmailTemplateDto
{
    public string Subject { get; private init; } = default!;
    public string Body { get; private init; } = default!;
}
