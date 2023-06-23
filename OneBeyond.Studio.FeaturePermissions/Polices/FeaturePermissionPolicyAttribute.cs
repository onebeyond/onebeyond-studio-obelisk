using OneBeyond.Studio.FeaturePermissions.Requirements;
using Sligo.Domain.SharedKernel.Authorization;

namespace OneBeyond.Studio.FeaturePermissions.Polices;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class FeaturePermissionPolicyAttribute : LocalBaseAuthorizationPolicyAttribute
{
    private static readonly Type _hasFeatureMembershipRequirement = typeof(HasFeaturePermissionRequirement);

    /// <summary>
    /// Defines an authorisation requirement for the signed-in user to have the features spcified
    /// </summary>
    /// <param name="feature">The group that the user needs to be enrolled within.</param>
    public FeaturePermissionPolicyAttribute(
        string feature)
        : base(_hasFeatureMembershipRequirement, new object[] { feature })
    {
    }

    /// <summary>
    /// Defines an authorisation requirement for the signed-in user to have one, or all of, the features specified.<br />
    /// By default, the user can be in any of the specified groups.
    /// </summary>
    /// <param name="features">The list of groups that the Sligo user needs to have.</param>
    public FeaturePermissionPolicyAttribute(
        params string[] features)
        : this(false, features)
    {
    }

    /// <summary>
    /// Defines an authorisation requirement for the signed-in user to have one, or all of, the features specified.<br />
    /// We can explicitly say whether or not the user needs to have all features, or just some.
    /// </summary>
    /// <param name="memberOfAll">Whether or not the user must have <strong>all</strong> specified features for this action.</param>
    /// <param name="features">The list of features that the user needs to have.</param>
    public FeaturePermissionPolicyAttribute(
        bool memberOfAll,
        params string[] features)
        : base(_hasFeatureMembershipRequirement, new object[] { features, memberOfAll ? MultiFeatureMemberRequirementType.MemberOfAll : MultiFeatureMemberRequirementType.MemberOfAny })
    {
    }

}


