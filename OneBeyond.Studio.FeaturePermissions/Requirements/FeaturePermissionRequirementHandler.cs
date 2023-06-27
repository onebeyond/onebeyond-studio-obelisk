using MediatR;
using OneBeyond.Studio.Application.SharedKernel.AmbientContexts;
using OneBeyond.Studio.Application.SharedKernel.Authorization;
using OneBeyond.Studio.FeaturePermissions.AmbientContexts;
using FeaturePermissionAmbientContext = OneBeyond.Studio.FeaturePermissions.AmbientContexts.FeaturePermissionAmbientContext;

namespace OneBeyond.Studio.FeaturePermissions.Requirements;
public class FeaturePermissionRequirementHandler<TRequest> : IAuthorizationRequirementHandler<HasFeaturePermissionRequirement, TRequest>
    where TRequest : IBaseRequest
{
    private readonly IAmbientContextAccessor<FeaturePermissionAmbientContext> _ambientContextAccessor;

    public FeaturePermissionRequirementHandler(
        IAmbientContextAccessor<FeaturePermissionAmbientContext> ambientContextAccessor)
    {
        _ambientContextAccessor = ambientContextAccessor;
    }

    public Task HandleAsync(HasFeaturePermissionRequirement requirement, TRequest request, CancellationToken cancellationToken)
    {
        bool authPassed;

        if (requirement.MembershipRequirementType == null
            || requirement.MembershipRequirementType == MultiFeatureMemberRequirementType.MemberOfAny)
        {
            authPassed = _ambientContextAccessor.UserIsInAnyGroup(requirement.Features);
        }
        else
        {
            authPassed = _ambientContextAccessor.UserIsInAllGroups(requirement.Features);
        }

        return authPassed ? Task.CompletedTask : throw new FeaturePermissionException("User does not have the required permissions");
    }
}
