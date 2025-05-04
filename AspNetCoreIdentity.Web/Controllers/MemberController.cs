using AspNetCoreIdentity.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreIdentity.Web.Controllers
{
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _SignInManager;

        public MemberController(SignInManager<AppUser> signInManager)
        {
            _SignInManager = signInManager;
        }

        public async Task Logout()
        {
            await _SignInManager.SignOutAsync();
        }
    }
}
