using AspNetCoreIdentity.Web.Extensions;
using AspNetCoreIdentity.Web.Models;
using AspNetCoreIdentity.Web.ViewModels;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using System.Security.Claims;

namespace AspNetCoreIdentity.Web.Controllers
{

    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _SignInManager;
        private readonly UserManager<AppUser> _UserManager;
        private readonly IFileProvider _fileProvider;
        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IFileProvider fileProvider)
        {
            _SignInManager = signInManager;
            _UserManager = userManager;
            _fileProvider = fileProvider;
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

		#region PasswordChange

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

		#endregion


		#region UserEdit 

        public async Task<IActionResult> UserEdit()
        {
            ViewBag.genderList = new SelectList(Enum.GetNames(typeof(Gender)));

            var currentUser = (await _UserManager.FindByNameAsync(User.Identity!.Name!))!;

            var userEditViewModel = new UserEditViewModel()
            {
                UserName = currentUser.UserName!,
                Email = currentUser.Email!,
                Phone = currentUser.PhoneNumber!,
                BirthDate = currentUser.BirthDate,
                City = currentUser.City,
                Gender = currentUser.Gender,
            };

            return View(userEditViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserEditViewModel request )
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            ViewBag.genderList = new SelectList(Enum.GetNames(typeof(Gender)));

            var currentUser = await _UserManager.FindByNameAsync(User.Identity!.Name!);

            currentUser.UserName = request.UserName;
            currentUser.Email = request.Email;
            currentUser.PhoneNumber = request.Phone;
            currentUser.BirthDate = request.BirthDate;
            currentUser.City = request.City;
            currentUser.Gender = request.Gender;

            if (request.Picture != null && request.Picture.Length > 0)
            {
                var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot");
                string randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(request.Picture.FileName)}";
                
                var newPicturePath = Path.Combine(wwwrootFolder!.First(x => x.Name == "userpictures").PhysicalPath!, randomFileName);
                using var stream = new FileStream(newPicturePath, FileMode.Create);
                await request.Picture.CopyToAsync(stream);

                currentUser.Picture = randomFileName;
            }

            var updateToUserResult = await _UserManager.UpdateAsync(currentUser);

            if (!updateToUserResult.Succeeded)
            {
                ModelState.AddModelErrorList(updateToUserResult.Errors);
                return View();
            }

            //Kritik bilgiler güncelledindiği için diğer uygulamalardan çıkış yapacak
            await _UserManager.UpdateSecurityStampAsync(currentUser);
            await _SignInManager.SignOutAsync();
            
            if (request.BirthDate.HasValue)
            {
                await _SignInManager.SignInWithClaimsAsync(currentUser, true, new[] { new Claim("birthdate", currentUser.BirthDate!.Value.ToString()) });
            }

            else
            {
                await _SignInManager.SignInAsync(currentUser, true);
            }

            TempData["SuccessMessage"] = "Üye bilgileri başarıyla değiştirilmiştir";

            var userEditViewModel = new UserEditViewModel()
            {
                UserName = currentUser.UserName!,
                Email = currentUser.Email!,
                Phone = currentUser.PhoneNumber!,
                BirthDate = currentUser.BirthDate,
                City = currentUser.City,
                Gender = currentUser.Gender,
            };

            return View(userEditViewModel);
        }

        #endregion

    }
}
