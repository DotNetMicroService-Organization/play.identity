#nullable disable

using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Play.Identity.Services.Entities;

namespace Play.Identity.Services.Areas.Identity.Pages.Account;

public class LogoutModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<LogoutModel> _logger;
    private readonly IIdentityServerInteractionService interaction;

    public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger, IIdentityServerInteractionService interaction)
    {
        _signInManager = signInManager;
        _logger = logger;
        this.interaction = interaction;
    }

    public async Task<IActionResult> OnGet(string logoutId)
    {
        var context = await interaction.GetLogoutContextAsync(logoutId);
        if (context?.ShowSignoutPrompt == false)
            return await this.OnPost(context.PostLogoutRedirectUri);

        return Page();
    }

    public async Task<IActionResult> OnPost(string returnUrl = null)
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out.");
        if (returnUrl != null)
        {
            return Redirect(returnUrl);
        }
        else
        {
            // This needs to be a redirect so that the browser performs a new
            // request and the identity for the user gets updated.
            return RedirectToPage();
        }
    }
}