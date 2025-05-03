using AspNetCoreIdentity.Web.Models;
using AspNetCoreIdentity.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics;

namespace AspNetCoreIdentity.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        //Readonly ile sadece oluþturulduðu anda veya constructor tarafýnda deðer atamasý yapýlabilir.
        //UserManager bizim için kullanýcý ile ilgili tüm iþlemleri yapar 
        private readonly UserManager<AppUser> _UserManager;

        public HomeController(ILogger<HomeController> logger , UserManager<AppUser> userManager)
        {
            _logger = logger;
            _UserManager = userManager;
        }

        #region SignUp
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel request)
        {

            if (!ModelState.IsValid)
                return View();

            AppUser appUser = new AppUser()
            {
                UserName = request.UserName,
                Email = request.Email,
                PhoneNumber = request.Phone,
            };

            var identityResult = await _UserManager.CreateAsync(appUser , request.Password);
            if (identityResult.Succeeded)
            {
                //Kullanýcý kaydý baþarýlý olmasý durumunda SuccessMessage kaybetmek istemiyoruz bu nedenle TempData içerisinde tutuyoruz
                TempData["SuccessMessage"] = "Kullanýcý baþarýyla oluþturuldu."; 
                return RedirectToAction(nameof(HomeController.SignUp));
            }

            //hata durumunda
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View();
        }
        #endregion

        #region Idnex Privacy Error

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #endregion
    }
}
