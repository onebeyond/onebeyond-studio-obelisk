using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.Extensions.Logging;
using OneBeyond.Studio.Crosscuts.Exceptions;
using OneBeyond.Studio.Crosscuts.Logging;
using OneBeyond.Studio.Crosscuts.Utilities.Templating;
using OneBeyond.Studio.Domain.SharedKernel.Repositories;
using OneBeyond.Studio.EmailProviders.Domain;
using OneBeyond.Studio.Obelisk.Application.Services.EmailTemplateService;
using OneBeyond.Studio.Obelisk.Domain.Exceptions;
using OneBeyond.Studio.Obelisk.Domain.Features.EmailTemplates.Entities;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Commands;
using OneBeyond.Studio.Obelisk.Domain.Features.Users.Entities;

namespace OneBeyond.Studio.Obelisk.Application.Features.Users.CommandHandlers;

internal sealed class SendResetPasswordEmailHandler : IRequestHandler<SendResetPasswordEmail, Unit>
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

    public async Task<Unit> Handle(SendResetPasswordEmail command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        try
        {
            var users = await _userRORepository.ListAsync(x => x.LoginId == command.LoginId, cancellationToken: cancellationToken).ConfigureAwait(false);

            var user = users.FirstOrDefault() ?? throw new ObeliskDomainException($"User with Login Id {command.LoginId} not found");

            var template = await _emailTemplateLoader.GetTemplateByKeyAsync(PredefinedEmailTemplates.RESET_PASSWORD).ConfigureAwait(false);

            var parameters = new
            {
                callbackUrl = command.ResetPasswordUrl,
                userName = user.UserName,
                name = user.UserName
            };

            var renderedBody = _templateRenderer.RenderTemplate(template.Body, parameters);

            await _mailSender.SendEmailAsync(user.Email, template.Subject, renderedBody, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        when (!exception.IsCritical())
        {
            Logger.LogError(exception, "Unable to send password reset email");
        }

        return Unit.Value;
    }
}
