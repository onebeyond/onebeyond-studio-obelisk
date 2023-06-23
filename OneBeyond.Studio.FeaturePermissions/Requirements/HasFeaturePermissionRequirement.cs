using OneBeyond.Studio.Domain.SharedKernel.Authorization;

namespace OneBeyond.Studio.FeaturePermissions.Requirements
{
    /// <summary>
    /// Ensures the signed in user is a member of the specified feature(s).
    /// </summary>
    public sealed record HasFeaturePermissionRequirement : AuthorizationRequirement
    {
        /// <summary>
        /// Creates a new authorization requirement that ensures the signed in user is a member of the specified features.
        /// </summary>
        /// <param name="features">All relevent features</param>
        /// <param name="multiFeatureMemberRequirement">Whether the user should be a member of any of the features or all of them</param>
        public HasFeaturePermissionRequirement(IEnumerable<string> features, MultiFeatureMemberRequirementType multiFeatureMemberRequirement)
        {
            Features = features;
            MembershipRequirementType = multiFeatureMemberRequirement;
        }

        /// <summary>
        /// Creates a new authorization requirement that ensures the signed in user is a member of the specified feature.
        /// </summary>
        /// <param name="feature">The feature</param>
        public HasFeaturePermissionRequirement(string feature)
        {
            Features = new[] { feature };
            MembershipRequirementType = null;
        }

        /// <summary>
        /// The features that make up this requirement
        /// </summary>
        public IEnumerable<string> Features { get; }

        /// <summary>
        /// Use this to control how the authorisation is performed when multiple features are required.<br />
        /// If multiple groups are set, we can either check to see if the signed-in user is a member of any of the features,
        /// or if they have to be a member of all of them for authorisation to succeed.
        /// </summary>
        public MultiFeatureMemberRequirementType? MembershipRequirementType { get; }

    }
}
