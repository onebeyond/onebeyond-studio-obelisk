namespace OneBeyond.Studio.Obelisk.Domain.Features.EmailTemplates.Entities;

public static class PredefinedEmailTemplates
{
    public const string ACCOUNT_SETUP = nameof(ACCOUNT_SETUP);
    public const string RESET_PASSWORD = nameof(RESET_PASSWORD);

    //Please note, those default templates are used to seed the database when the application is initialised.
    //After that they may be changed there.
    public static EmailTemplate DefaultAccountSetupEmailTemplate =>
        new
        (
            id: ACCOUNT_SETUP,
            description: "Account Setup",
            subject: "Set up your account for Obelisk",
            body: @"<html>
<body>
<p>Hello {{name}},</p>
<p>You have been sent this email as an invitation to access Obelisk. In order to access the system you will first need to set a password. Please click <a href=""{{callbackUrl}}"">here</a> to set your password.</p>
<p>If you're having trouble clicking the link, copy and paste the URL below into your web browser: {{callbackUrl}}.</p>
<p>To log in to your account use the following user name: {{userName}}.</p>
<p>Best Regards,<br />Obelisk Team</p>
</body></html>"
        );

    public static EmailTemplate DefaultPasswordResetEmailTemplate =>
        new
        (
            id: RESET_PASSWORD,
            description: "Password Reset",
            subject: $"Reset password for Obelisk user",
            body: @"<html>
<body>
<p>Hello {{name}},</p>
<p>You recently requested to reset your password for your Obelisk admin account. Click <a href=""{{callbackUrl}}"">here</a> to reset it.</p>
<p>If you did not request a password reset, please ignore this email or contact us.</p>
<p>If you're having trouble clicking the password rest link, copy and paste the URL below into your web browser: {{callbackUrl}}.</p>
<p>Best Regards,<br />Obelisk Team</p>
</body></html>"
        );
}
