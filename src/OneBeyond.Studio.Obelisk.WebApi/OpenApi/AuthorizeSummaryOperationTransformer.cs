using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace OneBeyond.Studio.Obelisk.WebApi.OpenApi;

/// <summary>
/// Inspects <see cref="AuthorizeAttribute"/> and <see cref="AllowAnonymousAttribute"/>
/// on each operation to:
/// <list type="bullet">
/// <item>
/// Append authorization metadata (roles, policies) to the operation summary.
/// </item>
/// <item>
/// Override the document-level security requirement with an empty array for 
/// anonymous endpoints, so they are not incorrectly shown as requiring a token.
/// </item>
/// </list>
/// </summary>
internal sealed class AuthorizeSummaryOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (context.Description.ActionDescriptor is not ControllerActionDescriptor descriptor)
        {
            return Task.CompletedTask;
        }

        var methodAttributes = descriptor.MethodInfo.GetCustomAttributes(inherit: true);
        var controllerAttributes = descriptor.ControllerTypeInfo.GetCustomAttributes(inherit: true);

        // If [AllowAnonymous] is present on the action or the controller, mark the
        // operation as not requiring authentication (overrides document-level security).
        if (methodAttributes.OfType<AllowAnonymousAttribute>().Any()
            || controllerAttributes.OfType<AllowAnonymousAttribute>().Any())
        {
            operation.Security = [];
            return Task.CompletedTask;
        }

        var authorizeAttributes = methodAttributes
            .OfType<AuthorizeAttribute>()
            .Concat(controllerAttributes.OfType<AuthorizeAttribute>())
            .ToList();

        if (authorizeAttributes.Count is 0)
        {
            // No explicit authorization requirement — override the document-level security
            // so anonymous endpoints are not incorrectly shown as requiring a token.
            operation.Security = [];
            return Task.CompletedTask;
        }

        // Build a human-readable auth summary suffix
        var parts = new List<string>();

        var roles = authorizeAttributes
            .Where(a => !string.IsNullOrEmpty(a.Roles))
            .SelectMany(a => a.Roles!.Split(',', StringSplitOptions.TrimEntries))
            .Distinct()
            .ToList();

        if (roles.Count > 0)
        {
            parts.Add($"Roles: {string.Join(", ", roles)}");
        }

        var policies = authorizeAttributes
            .Where(a => !string.IsNullOrEmpty(a.Policy))
            .Select(a => a.Policy!)
            .Distinct()
            .ToList();

        if (policies.Count > 0)
        {
            parts.Add($"Policies: {string.Join(", ", policies)}");
        }

        if (parts.Count > 0)
        {
            var suffix = $" (Auth: {string.Join("; ", parts)})";
            operation.Summary = (operation.Summary ?? string.Empty) + suffix;
        }
        else
        {
            operation.Summary = (operation.Summary ?? string.Empty) + " (Auth)";
        }

        return Task.CompletedTask;
    }
}
