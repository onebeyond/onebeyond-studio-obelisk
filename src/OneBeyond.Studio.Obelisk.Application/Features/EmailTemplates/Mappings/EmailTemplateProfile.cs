using OneBeyond.Studio.Obelisk.Application.Services.EmailTemplateService;
using OneBeyond.Studio.Obelisk.Domain.Features.EmailTemplates.Entities;

namespace OneBeyond.Studio.Obelisk.Application.Features.EmailTemplates.Mappings;

internal sealed class EmailTemplateProfile : AutoMapper.Profile
{
    public EmailTemplateProfile()
    {
        CreateMap<EmailTemplate, EmailTemplateDto>()
            .ForMember(
                dto => dto.Subject,
                opt => opt.MapFrom(emailTemplate => emailTemplate.Subject))
            .ForMember(
                dto => dto.Body,
                opt => opt.MapFrom(emailTemplate => emailTemplate.Body));
    }
}
