using System.Threading.Tasks;

namespace OneBeyond.Studio.Obelisk.Application.Services.EmailTemplateService;

internal interface IEmailTemplateService
{
    Task<EmailTemplateDto> GetTemplateByKeyAsync(string key);
}
