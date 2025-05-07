using AspNetCoreIdentity.Web.Models;
using AspNetCoreIdentity.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreIdentity.Web.Controllers
{

    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _SignInManager;
        private readonly UserManager<AppUser> _UserManager;

        public MemberController(SignInManager<AppUser> signInManager , UserManager<AppUser> userManager)
        {
            _SignInManager = signInManager;
            _UserManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _UserManager.FindByNameAsync(User.Identity!.Name);

            var userViewModel = new UserViewModel
            {
                UserName = currentUser.UserName,
                Email = currentUser.Email,
                PhoneNumber = currentUser.PhoneNumber,
                //PictureUrl = currentUser.PictureUrl
            };

            return View(userViewModel);
        }

        public async Task Logout()
        {
            await _SignInManager.SignOutAsync();
        }


    }
}
