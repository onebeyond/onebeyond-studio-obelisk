using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.Application.SharedKernel.DomainEvents;
using OneBeyond.Studio.Application.SharedKernel.Repositories;
using OneBeyond.Studio.Crosscuts.Exceptions;
using OneBeyond.Studio.Crosscuts.Logging;
using OneBeyond.Studio.Crosscuts.Utilities.Templating;
using OneBeyond.Studio.EmailProviders.Domain;
using OneBeyond.Studio.Obelisk.Application.Services.EmailTemplateService;
using OneBeyond.Studio.Obelisk.Domain.Features.EmailTemplates.Entities;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.DomainEvents;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;

namespace OneBeyond.Studio.Obelisk.Application.Features.Users.DomainEventHandlers;

internal sealed class NewUserPasswordEmailSender : IPostSaveDomainEventHandler<UserCreated>
{
    private static readonly ILogger Logger = LogManager.CreateLogger<NewUserPasswordEmailSender>();

    private readonly IRORepository<UserBase, Guid> _userRORepository;
    private readonly IEmailTemplateService _emailTemplateLoader;
    private readonly ITemplateRenderer _templateRenderer;
    private readonly IEmailSender _mailSender;

    public NewUserPasswordEmailSender(
        IRORepository<UserBase, Guid> userRORepository,
        IEmailTemplateService emailTemplateLoader,
        ITemplateRenderer templateRenderer,
        IEmailSender mailSender)
    {
        EnsureArg.IsNotNull(userRORepository, nameof(userRORepository));
        EnsureArg.IsNotNull(emailTemplateLoader, nameof(emailTemplateLoader));
        EnsureArg.IsNotNull(templateRenderer, nameof(templateRenderer));
        EnsureArg.IsNotNull(mailSender, nameof(mailSender));

        _userRORepository = userRORepository;
        _emailTemplateLoader = emailTemplateLoader;
        _templateRenderer = templateRenderer;
        _mailSender = mailSender;
    }

    public async Task HandleAsync(
        UserCreated domainEvent,
        IReadOnlyDictionary<string, object> domainEventAmbientContext,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(domainEvent, nameof(domainEvent));

        if (domainEvent.ResetPasswordUrl == null)
        {
            //For some users (for example, for those, who were created for external sing ins (Azure AD))
            //reset password url feature is not supported
            return;
        }

        var user = await _userRORepository.GetByIdAsync(domainEvent.UserId, cancellationToken: cancellationToken);

        await SendEmailSync(user, domainEvent.ResetPasswordUrl, cancellationToken);
    }

    private async Task SendEmailSync(
        UserBase user,
        Uri passwordResetUrl,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(user, nameof(user));

        try
        {
            var template = await _emailTemplateLoader.GetTemplateByKeyAsync(PredefinedEmailTemplates.ACCOUNT_SETUP);

            var parameters = new
            {
                template.Subject,
                CallBackUrl = passwordResetUrl,
                Name = user.UserName,
                user.UserName
            };

            var renderedBody = _templateRenderer.Render(template.Body, parameters);

            await _mailSender.SendEmailAsync(user.Email, template.Subject, renderedBody, cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        when (!exception.IsCritical())
        {
            Logger.LogError(exception, "Unable to create user password reset/setup email");
        }
    }

}
