using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OneBeyond.Studio.Application.SharedKernel.AmbientContexts;
using OneBeyond.Studio.Obelisk.Application.Services.AmbientContexts;
using AmbientContext = OneBeyond.Studio.Obelisk.Application.Services.AmbientContexts.AmbientContext;

namespace OneBeyond.Studio.Obelisk.WebApi.Pages;

[Authorize]
public sealed class IndexModel : PageModel
{
    public IndexModel(IAmbientContextAccessor<AmbientContext> ambientContextAccessor)
    {
        EnsureArg.IsNotNull(ambientContextAccessor, nameof(ambientContextAccessor));

        UserContext = ambientContextAccessor.AmbientContext.GetUserContext();
    }

    public UserContext UserContext { get; }

    public Task OnGetAsync(CancellationToken _)
        => Task.CompletedTask;
}
