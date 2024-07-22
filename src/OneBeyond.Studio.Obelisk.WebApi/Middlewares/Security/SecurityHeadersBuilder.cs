using EnsureThat;
using Microsoft.Extensions.Configuration;
using static OneBeyond.Studio.Obelisk.WebApi.Middlewares.Security.Constants.SecurityMiddlewareConstants;

namespace OneBeyond.Studio.Obelisk.WebApi.Middlewares.Security;

internal sealed class SecurityHeadersBuilder
{
    private readonly SecurityHeadersPolicy _policy;

    public SecurityHeadersBuilder()
    {
        _policy = new SecurityHeadersPolicy();
    }

    public SecurityHeadersBuilder AddSecurityPolicyFromConfiguration(
        IConfigurationSection securityHeaders)
    {
        EnsureArg.IsNotNull(securityHeaders, nameof(securityHeaders));

        AddContentTypeOptionsNoSniff();

        var headerValue = securityHeaders.GetValue<string>(AccessControlExposeHeadersConstants.Header);
        if (!string.IsNullOrEmpty(headerValue))
        {
            AddCustomHeader(AccessControlExposeHeadersConstants.Header, headerValue);
        }

        headerValue = securityHeaders.GetValue<string>(FrameOptionsConstants.Header);
        if (string.IsNullOrEmpty(headerValue))
        {
            AddFrameOptionsSameOrigin();
        }
        else
        {
            AddCustomHeader(FrameOptionsConstants.Header, headerValue);
        }

        headerValue = securityHeaders.GetValue<string>(XssProtectionConstants.Header);
        if (string.IsNullOrEmpty(headerValue))
        {
            AddXssProtectionBlock();
        }
        else
        {
            AddCustomHeader(XssProtectionConstants.Header, headerValue);
        }

        headerValue = securityHeaders.GetValue<string>(StrictTransportSecurityConstants.Header);
        if (string.IsNullOrEmpty(headerValue))
        {
            AddStrictTransportSecurityMaxAge();
        }
        else
        {
            AddCustomHeader(StrictTransportSecurityConstants.Header, headerValue);
        }

        headerValue = securityHeaders.GetValue<string>(XPermittedCrossDomainPoliciesConstants.Header);
        if (string.IsNullOrEmpty(headerValue))
        {
            AddXPermittedCrossDomainPoliciesNone();
        }
        else
        {
            AddCustomHeader(XPermittedCrossDomainPoliciesConstants.Header, headerValue);
        }

        return this;
    }

    public SecurityHeadersBuilder AddDefaultSecurePolicy()
    {
        AddFrameOptionsDeny();
        AddXssProtectionBlock();
        AddContentTypeOptionsNoSniff();
        AddStrictTransportSecurityMaxAge();
        AddXPermittedCrossDomainPoliciesNone();

        return this;
    }

    public SecurityHeadersBuilder AddFrameOptionsDeny()
    {
        _policy.SetHeaders[FrameOptionsConstants.Header] = FrameOptionsConstants.Deny;
        return this;
    }

    public SecurityHeadersBuilder AddFrameOptionsSameOrigin()
    {
        _policy.SetHeaders[FrameOptionsConstants.Header] = FrameOptionsConstants.SameOrigin;
        return this;
    }

    public SecurityHeadersBuilder AddFrameOptionsSameOrigin(string uri)
    {
        _policy.SetHeaders[FrameOptionsConstants.Header] = string.Format(FrameOptionsConstants.AllowFromUri, uri);
        return this;
    }

    public SecurityHeadersBuilder AddCustomHeader(string header, string value)
    {
        _policy.SetHeaders[header] = value;
        return this;
    }

    public SecurityHeadersBuilder RemoveHeader(string header)
    {
        _policy.RemoveHeaders.Add(header);
        return this;
    }

    public SecurityHeadersPolicy Build()
        => _policy;

    public void AddXssProtectionBlock()
        => AddCustomHeader(XssProtectionConstants.Header, XssProtectionConstants.Enabled);

    public void AddContentTypeOptionsNoSniff()
        => AddCustomHeader(ContentTypeOptionsConstants.Header, ContentTypeOptionsConstants.NoSniff);

    public void AddStrictTransportSecurityMaxAge()
        => AddCustomHeader(StrictTransportSecurityConstants.Header, StrictTransportSecurityConstants.MaxYear);

    public void AddXPermittedCrossDomainPoliciesNone()
        => AddCustomHeader(XPermittedCrossDomainPoliciesConstants.Header, XPermittedCrossDomainPoliciesConstants.None);

    public void AddContentSecurityPolicy()
        => AddCustomHeader(ContentSecurityPolicyConstants.Header, ContentSecurityPolicyConstants.Self);
}
