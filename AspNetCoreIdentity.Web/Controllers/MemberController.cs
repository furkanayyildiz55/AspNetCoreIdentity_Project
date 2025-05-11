using AspNetCoreIdentity.Web.Extensions;
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

        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
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

        public async Task<IActionResult> PasswordChange()
        {
            return View();
        }

        [HttpPost]
		public async Task<IActionResult> PasswordChange(PasswordChangeViewModel request)
		{
            if (!ModelState.IsValid)
                return View();

            var currentUser = await _UserManager.FindByNameAsync(User.Identity!.Name);
			var checkOldPassword = await _UserManager.CheckPasswordAsync(currentUser, request.PasswordOld);

			if (!checkOldPassword)
			{
				ModelState.AddModelError(string.Empty, "Eski şifreniz yanlış");
				return View();
			}

			var resultChangePassword = await _UserManager.ChangePasswordAsync(currentUser, request.PasswordOld, request.PasswordNew);

            if (!resultChangePassword.Succeeded)
            {
				ModelState.AddModelErrorList(resultChangePassword.Errors.Select(x => x.Description).ToList());
				return View();
			}

			//Security stamp güncellenerek diğer cihazlardan çıkış yapılır
			await _UserManager.UpdateSecurityStampAsync(currentUser);

			//Çıkış ve ardından tekrar giriş yapma işlemi ile cookie güncellenir
			await _SignInManager.SignOutAsync();
            await _SignInManager.PasswordSignInAsync(currentUser, request.PasswordNew, true, false);

			TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirilmiştir";
			return View();
		}


	}
}
