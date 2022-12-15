using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OneBeyond.Studio.Obelisk.WebApi.Extensions;

namespace OneBeyond.Studio.Obelisk.WebApi.Pages.Account.TFA;

[Authorize]
public sealed class ShowRecoveryCodesModel : PageModel
{
    [TempData]
    public string[] RecoveryCodes { get; set; } = Array.Empty<string>();

    [TempData]
    public string? StatusMessage { get; set; }

    public IActionResult OnGet()
        => RecoveryCodes == null || RecoveryCodes.Length == 0
            ? RedirectToPage("./TwoFactorAuthentication")
            : Page();

    public IActionResult OnPostAsync()
        => LocalRedirect(Url.GetReturnUrl("/"));
}
