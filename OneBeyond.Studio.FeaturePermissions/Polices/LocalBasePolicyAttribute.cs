using EnsureThat;
using MoreLinq;
using OneBeyond.Studio.Domain.SharedKernel.Authorization;

namespace Sligo.Domain.SharedKernel.Authorization;

/// <summary>
/// Base AuthorizationPolicyAttribute from <see cref="OneBeyond.Studio.Core.Domain.SharedKernel"/>
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class LocalBaseAuthorizationPolicyAttribute : Attribute
{
    /// <summary>
    /// Creates a new authorization policy with a list of requirements.
    /// </summary>
    /// <param name="requirementTypes"></param>
    public LocalBaseAuthorizationPolicyAttribute(params Type[] requirementTypes)
    {
        EnsureArg.HasItems(requirementTypes, nameof(requirementTypes));

        RequirementTypes = requirementTypes.ToDictionary(
            (requirementType) => requirementType,
            (requirementType) => Array.Empty<object>() as IReadOnlyCollection<object>);

        EnsureValidRequirementTypes(RequirementTypes);
    }

    /// <summary>
    /// Creates an authorization policy with one requirement and its required arguments.
    /// </summary>
    /// <param name="requirement1Type"></param>
    /// <param name="requirement1Args">
    /// Arguments to be used for initializing the requirement. As soon as attributes support just primitive types,
    /// one can use a JSON string or configuration section name to get some more complex data inside the requirement.
    /// </param>
    public LocalBaseAuthorizationPolicyAttribute(
        Type requirement1Type,
        object[] requirement1Args)
    {
        EnsureArg.IsNotNull(requirement1Type, nameof(requirement1Type));
        EnsureArg.IsNotNull(requirement1Args, nameof(requirement1Args));

        RequirementTypes = new Dictionary<Type, IReadOnlyCollection<object>>
            {
                { requirement1Type, requirement1Args }
            };

        EnsureValidRequirementTypes(RequirementTypes);
    }

    /// <summary>
    /// Creates an authorization policy with two requirements and their respective required arguments.
    /// </summary>
    /// <param name="requirement1Type"></param>
    /// <param name="requirement1Args"></param>
    /// <param name="requirement2Type"></param>
    /// <param name="requirement2Args"></param>
    public LocalBaseAuthorizationPolicyAttribute(
        Type requirement1Type,
        object[] requirement1Args,
        Type requirement2Type,
        object[] requirement2Args)
    {
        EnsureArg.IsNotNull(requirement1Type, nameof(requirement1Type));
        EnsureArg.IsNotNull(requirement1Args, nameof(requirement1Args));
        EnsureArg.IsNotNull(requirement2Type, nameof(requirement2Type));
        EnsureArg.IsNotNull(requirement2Args, nameof(requirement2Args));

        RequirementTypes = new Dictionary<Type, IReadOnlyCollection<object>>
            {
                { requirement1Type, requirement1Args },
                { requirement2Type, requirement2Args }
            };

        EnsureValidRequirementTypes(RequirementTypes);
    }

    /// <summary>
    /// Creates a new authorization policy with three requirements and their respective required arguments.
    /// </summary>
    /// <param name="requirement1Type"></param>
    /// <param name="requirement1Args"></param>
    /// <param name="requirement2Type"></param>
    /// <param name="requirement2Args"></param>
    /// <param name="requirement3Type"></param>
    /// <param name="requirement3Args"></param>
    public LocalBaseAuthorizationPolicyAttribute(
        Type requirement1Type,
        object[] requirement1Args,
        Type requirement2Type,
        object[] requirement2Args,
        Type requirement3Type,
        object[] requirement3Args)
    {
        EnsureArg.IsNotNull(requirement1Type, nameof(requirement1Type));
        EnsureArg.IsNotNull(requirement1Args, nameof(requirement1Args));
        EnsureArg.IsNotNull(requirement2Type, nameof(requirement2Type));
        EnsureArg.IsNotNull(requirement2Args, nameof(requirement2Args));
        EnsureArg.IsNotNull(requirement3Type, nameof(requirement3Type));
        EnsureArg.IsNotNull(requirement3Args, nameof(requirement3Args));

        RequirementTypes = new Dictionary<Type, IReadOnlyCollection<object>>
            {
                { requirement1Type, requirement1Args },
                { requirement2Type, requirement2Args },
                { requirement3Type, requirement3Args }
            };

        EnsureValidRequirementTypes(RequirementTypes);
    }

    /// <summary>
    /// A list of requirement types.
    /// </summary>
    public IReadOnlyDictionary<Type, IReadOnlyCollection<object>> RequirementTypes { get; }

    private static void EnsureValidRequirementTypes(IReadOnlyDictionary<Type, IReadOnlyCollection<object>> requirementTypes)
    {
        requirementTypes
            .Assert(
                (requirementType) => typeof(AuthorizationRequirement).IsAssignableFrom(requirementType.Key)
                    && !requirementType.Key.IsAbstract,
                (requirementType) => throw new ArgumentException(
                    $"Authorization requirement type {requirementType.Key} is not assignable from {typeof(AuthorizationRequirement)}.",
                    nameof(requirementType)))
            .Assert(
                (requirementType) => requirementType.Key.GetConstructor(
                    requirementType.Value.Select((value) => value.GetType()).ToArray()) is not null,
                (requirementType) => throw new ArgumentException(
                    $"Authorization requirement type {requirementType.Key} does not have constructor matching to specified arguments.",
                    nameof(requirementType)))
            .Consume();
    }
}
