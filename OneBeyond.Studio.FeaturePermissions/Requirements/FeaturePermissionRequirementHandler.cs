using MediatR;
using OneBeyond.Studio.Application.SharedKernel.AmbientContexts;
using OneBeyond.Studio.Application.SharedKernel.Authorization;
using OneBeyond.Studio.FeaturePermissions.AmbientContexts;
using AmbientContext = OneBeyond.Studio.FeaturePermissions.AmbientContexts.AmbientContext;

namespace OneBeyond.Studio.FeaturePermissions.Requirements;
public class FeaturePermissionRequirementHandler<TRequest> : IAuthorizationRequirementHandler<HasFeaturePermissionRequirement, TRequest>
    where TRequest : IBaseRequest
{
    private readonly IAmbientContextAccessor<AmbientContext> _ambientContextAccessor;

    public FeaturePermissionRequirementHandler(
        IAmbientContextAccessor<AmbientContext> ambientContextAccessor)
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
