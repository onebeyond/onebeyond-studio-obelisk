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
    private const string DefaultFolderPath = "Features.EmailTemplates.Templates";

    public const string LAYOUT = nameof(LAYOUT);
    public const string ACCOUNT_SETUP = nameof(ACCOUNT_SETUP);
    public const string RESET_PASSWORD = nameof(RESET_PASSWORD);

    private static readonly Assembly _assembly = typeof(PredefinedEmailTemplates).Assembly;
    private static readonly string _assemblyName = _assembly.GetName().Name!;

    private static string GetManifestResourceName(string path)
    {
        return _assembly.GetManifestResourceNames()
            .FirstOrDefault(resourceName => path.Equals(resourceName, StringComparison.OrdinalIgnoreCase))
            ?? throw new FileNotFoundException($"Could not find resource: '{path}'.", path);
    }

    private static Stream GetManifestResourceStream(string path)
        => _assembly.GetManifestResourceStream(path) ?? throw new UnauthorizedAccessException($"Could not read resource '{path}'.");

    private static string GetTemplateBody(string filePath)
    {
        EnsureArg.IsNotNullOrWhiteSpace(filePath, nameof(filePath));

        var path = GetManifestResourceName($"{_assemblyName}.{filePath}");

        using var stream = GetManifestResourceStream(path);
        using var streamReader = new StreamReader(stream);
        return streamReader.ReadToEnd();
    }

    private static string GetLayoutTemplateBody(
        string filePath,
        string logoPath = $"{DefaultFolderPath}.logo.webp",
        string mediaType = MediaTypeNames.Image.Webp)
    {
        var body = GetTemplateBody(filePath);

        var path = GetManifestResourceName($"{_assemblyName}.{logoPath}");

        using var stream = GetManifestResourceStream(path);
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);

        var logoContent = memoryStream.ToArray();
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
            body: GetLayoutTemplateBody($"{DefaultFolderPath}.{LAYOUT}.html")
        );


    public static EmailTemplate DefaultAccountSetupEmailTemplate =>
        new EmailTemplate
        (
            id: ACCOUNT_SETUP,
            description: "Account Setup",
            subject: "Set up your account for Obelisk",
            body: GetTemplateBody($"{DefaultFolderPath}.{ACCOUNT_SETUP}.html")
        );

    public static EmailTemplate DefaultPasswordResetEmailTemplate =>
        new EmailTemplate
        (
            id: RESET_PASSWORD,
            description: "Password Reset",
            subject: $"Reset password for Obelisk user",
            body: GetTemplateBody($"{DefaultFolderPath}.{RESET_PASSWORD}.html")
        );
}
