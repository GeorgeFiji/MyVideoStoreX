using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace MyVideostore.Controllers
{
    [Area("Identity")]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return RedirectToPage("/Account/Register", new { area = "Identity" });
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Account/Logout", new { area = "Identity" });
        }
    }
}
