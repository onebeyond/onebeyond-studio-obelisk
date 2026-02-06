using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.Application.SharedKernel.Repositories;
using OneBeyond.Studio.Core.Mediator;
using OneBeyond.Studio.Crosscuts.Exceptions;
using OneBeyond.Studio.Crosscuts.Logging;
using OneBeyond.Studio.EmailProviders.Domain;
using OneBeyond.Studio.Obelisk.Application.Services.EmailTemplateService;
using OneBeyond.Studio.Obelisk.Domain.Exceptions;
using OneBeyond.Studio.Obelisk.Domain.Features.EmailTemplates.Entities;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Commands;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;
using OneBeyond.Studio.TemplateRendering;

namespace OneBeyond.Studio.Obelisk.Application.Features.Users.CommandHandlers;

internal sealed class SendResetPasswordEmailHandler : IRequestHandler<SendResetPasswordEmail>
{
    private static readonly ILogger Logger = LogManager.CreateLogger<SendResetPasswordEmailHandler>();

    private readonly IRORepository<UserBase, Guid> _userRORepository;

    private readonly IEmailSender _mailSender;
    private readonly ITemplateRenderer _templateRenderer;
    private readonly IEmailTemplateService _emailTemplateLoader;

    public SendResetPasswordEmailHandler(
        IRORepository<UserBase, Guid> userRORepository,
        IEmailSender mailSender,
        ITemplateRenderer templateRenderer,
        IEmailTemplateService emailTemplateLoader)
    {
        EnsureArg.IsNotNull(userRORepository, nameof(userRORepository));
        EnsureArg.IsNotNull(emailTemplateLoader, nameof(emailTemplateLoader));
        EnsureArg.IsNotNull(templateRenderer, nameof(templateRenderer));
        EnsureArg.IsNotNull(mailSender, nameof(mailSender));

        _userRORepository = userRORepository;
        _mailSender = mailSender;
        _templateRenderer = templateRenderer;
        _emailTemplateLoader = emailTemplateLoader;
    }

    public async Task Handle(SendResetPasswordEmail command, CancellationToken cancellationToken = default)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        try
        {
            var users = await _userRORepository.ListAsync(user => user.LoginId == command.LoginId, cancellationToken: cancellationToken);

            var user = users.FirstOrDefault() ?? throw new ObeliskDomainException($"User with Login Id {command.LoginId} not found");

            var template = await _emailTemplateLoader.GetTemplateByKeyAsync(PredefinedEmailTemplates.RESET_PASSWORD);

            var parameters = new
            {
                template.Subject,
                CallBackUrl = command.ResetPasswordUrl,
                user.UserName,
                Name = user.UserName
            };

            var renderedBody = _templateRenderer.Render(template.Body, parameters);

            await _mailSender.SendEmailAsync(user.Email, template.Subject, renderedBody, cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        when (!exception.IsCritical())
        {
            Logger.LogError(exception, "Unable to send password reset email");
        }
    }
}
