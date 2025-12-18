namespace OneBeyond.Studio.Obelisk.Application.Services.EmailTemplateService;

public sealed record EmailTemplateDto
{
    public string Subject { get; init; } = default!;
    public string Body { get; init; } = default!;
}
