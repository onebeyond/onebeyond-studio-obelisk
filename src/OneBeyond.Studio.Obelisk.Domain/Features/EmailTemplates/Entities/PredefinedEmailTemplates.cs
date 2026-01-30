using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using EnsureThat;

namespace OneBeyond.Studio.Obelisk.Domain.Features.EmailTemplates.Entities;

/// <remarks>
/// Default templates are used to seed the database when the application is initialised.
/// After that they may be changed there.
/// </remarks>
public static class PredefinedEmailTemplates
{
    private const string DefaultDirectory = "Features/EmailTemplates/Templates";

    public const string LAYOUT = nameof(LAYOUT);
    public const string ACCOUNT_SETUP = nameof(ACCOUNT_SETUP);
    public const string RESET_PASSWORD = nameof(RESET_PASSWORD);

    private static string GetTemplateBody(string fileName, string directory = DefaultDirectory)
    {
        EnsureArg.IsNotNullOrWhiteSpace(fileName, nameof(fileName));
        EnsureArg.IsNotNullOrWhiteSpace(directory, nameof(directory));

        fileName = fileName.ToLowerInvariant();
        var templatePath = Path.Combine(AppContext.BaseDirectory, directory, fileName);
        return File.ReadAllText(templatePath);
    }

    private static string GetLayoutTemplateBody(
        string fileName,
        string directory = DefaultDirectory,
        string logoPath = "Features/EmailTemplates/Templates/logo.webp",
        string mediaType = MediaTypeNames.Image.Webp)
    {
        var body = GetTemplateBody(fileName, directory);

        logoPath = Path.Combine(AppContext.BaseDirectory, logoPath);
        var logoContent = File.ReadAllBytes(logoPath);
        var encodedLogo = Convert.ToBase64String(logoContent);

        return body.Replace("{{{Logo}}}", $"data:{mediaType};base64,{encodedLogo}");
    }


    public static IEnumerable<EmailTemplate> All =>
        typeof(PredefinedEmailTemplates)
            .GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(property => typeof(EmailTemplate).IsAssignableFrom(property.PropertyType))
            .Select(property => property.GetValue(null))
            .OfType<EmailTemplate>();


    public static EmailTemplate DefaultEmailLayoutTemplate =>
        new EmailTemplate
        (
            id: LAYOUT,
            description: "Email layout",
            subject: "dummy",
            body: GetLayoutTemplateBody($"{LAYOUT}.html")
        );


    public static EmailTemplate DefaultAccountSetupEmailTemplate =>
        new EmailTemplate
        (
            id: ACCOUNT_SETUP,
            description: "Account Setup",
            subject: "Set up your account for Obelisk",
            body: GetTemplateBody($"{ACCOUNT_SETUP}.html")
        );

    public static EmailTemplate DefaultPasswordResetEmailTemplate =>
        new EmailTemplate
        (
            id: RESET_PASSWORD,
            description: "Password Reset",
            subject: $"Reset password for Obelisk user",
            body: GetTemplateBody($"{RESET_PASSWORD}.html")
        );
}
