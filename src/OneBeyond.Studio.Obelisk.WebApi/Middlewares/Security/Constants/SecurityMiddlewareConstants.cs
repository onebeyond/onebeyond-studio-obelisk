namespace OneBeyond.Studio.Obelisk.WebApi.Middlewares.Security.Constants;

public static class SecurityMiddlewareConstants
{
    public static class AccessControlExposeHeadersConstants
    {
        /// <summary>
        /// Header value for Access-Control-Expose-Headers
        /// </summary>
        public const string Header = "Access-Control-Expose-Headers";

        public const string ContentDisposition = "Content-Disposition";
    }

    public static class ContentSecurityPolicyConstants
    {
        /// <summary>
        /// Header value for content-security-policy
        /// </summary>
        public const string Header = "Content-Security-Policy";

        /// <summary>
        /// Disables content sniffing
        /// </summary>
        public const string Self = "default-src 'self'";
    }

    public static class ContentTypeOptionsConstants
    {
        /// <summary>
        /// Header value for X-Content-Type-Options
        /// </summary>
        public const string Header = "X-Content-Type-Options";

        /// <summary>
        /// Disables content sniffing
        /// </summary>
        public const string NoSniff = "nosniff";
    }

    public static class FrameOptionsConstants
    {
        /// <summary>
        /// The header value for X-Frame-Options
        /// </summary>
        public const string Header = "X-Frame-Options";

        /// <summary>
        /// The page cannot be displayed in a frame, regardless of the site attempting to do so.
        /// </summary>
        public const string Deny = "DENY";

        /// <summary>
        /// The page can only be displayed in a frame on the same origin as the page itself.
        /// </summary>
        public const string SameOrigin = "SAMEORIGIN";

        /// <summary>
        /// The page can only be displayed in a frame on the specified origin. {0} specifies the format string
        /// </summary>
        public const string AllowFromUri = "ALLOW-FROM {0}";
    }

    public static class StrictTransportSecurityConstants
    {
        /// <summary>
        /// Header value for Strict-Transport-Security
        /// </summary>
        public const string Header = "Strict-Transport-Security";

        /// <summary>
        /// Tells the user-agent to cache the domain in the STS list for the provided number of seconds {0} 
        /// </summary>
        public const string MaxAge = "max-age={0}";


        /// <summary>
        /// Tells the user-agent to cache the domain in the STS list for a year
        /// </summary>
        public const string MaxYear = "max-age=31536000";

        /// <summary>
        /// Tells the user-agent to cache the domain in the STS list for the provided number of seconds {0} and include any sub-domains.
        /// </summary>
        public const string MaxAgeIncludeSubdomains = "max-age={0}; includeSubDomains";

        /// <summary>
        /// Tells the user-agent to remove, or not cache the host in the STS cache.
        /// </summary>
        public const string NoCache = "max-age=0";
    }

    public static class XPermittedCrossDomainPoliciesConstants
    {
        /// <summary>
        /// Header value for x-permitted-cross-domain-policies
        /// </summary>
        public const string Header = "X-Permitted-Cross-Domain-Policies";

        /// <summary>
        /// Disables content sniffing
        /// </summary>
        public const string None = "none";
    }

    public static class XssProtectionConstants
    {

        /// <summary>
        /// Header value for X-XSS-Protection
        /// </summary>
        public const string Header = "X-XSS-Protection";

        /// <summary>
        /// Enables the XSS Protections
        /// </summary>
        public const string Enabled = "1";

        /// <summary>
        /// Disables the XSS Protections offered by the user-agent.
        /// </summary>
        public const string Disabled = "0";

        /// <summary>
        /// Enables XSS protections and instructs the user-agent to block the response in the event that script has been inserted from user input, instead of sanitizing.
        /// </summary>
        public const string Block = "1; mode=block";

        /// <summary>
        /// A partially supported directive that tells the user-agent to report potential XSS attacks to a single URL. Data will be POST'd to the report URL in JSON format. 
        /// {0} specifies the report url, including protocol
        /// </summary>
        public const string Report = "1; report={0}";
    }

}
