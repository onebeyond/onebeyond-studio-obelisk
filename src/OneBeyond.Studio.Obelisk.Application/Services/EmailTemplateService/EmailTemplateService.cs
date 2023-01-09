using System.Linq;
using System.Threading.Tasks;
using EnsureThat;
using OneBeyond.Studio.Application.SharedKernel.Repositories;
using OneBeyond.Studio.Obelisk.Domain.Exceptions;
using OneBeyond.Studio.Obelisk.Domain.Features.EmailTemplates.Entities;

namespace OneBeyond.Studio.Obelisk.Application.Services.EmailTemplateService;

internal sealed class EmailTemplateService : IEmailTemplateService
{
    private readonly IRORepository<EmailTemplate, string> _roRepository;

    public EmailTemplateService(IRORepository<EmailTemplate, string> roRepository)
    {
        EnsureArg.IsNotNull(roRepository, nameof(roRepository));

        _roRepository = roRepository;
    }

    public async Task<EmailTemplateDto> GetTemplateByKeyAsync(string key)
    {
        return (await _roRepository
                .ListAsync<EmailTemplateDto>((template) => template.Id == key)
                .ConfigureAwait(false))
            .SingleOrDefault()
            ?? throw new ObeliskDomainException($"Unable to find template with key {key}");
    }
}
