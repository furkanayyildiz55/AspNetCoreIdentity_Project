using AspNetCoreIdentity.Web.Extensions;
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

        //Readonly ile sadece olu�turuldu�u anda veya constructor taraf�nda de�er atamas� yap�labilir.
        //UserManager bizim i�in kullan�c� ile ilgili t�m i�lemleri yapar 
        private readonly UserManager<AppUser> _UserManager;

        private readonly SignInManager<AppUser> _SignInManager;

        public HomeController(ILogger<HomeController> logger , UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _UserManager = userManager;
            _SignInManager = signInManager;
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
                //Kullan�c� kayd� ba�ar�l� olmas� durumunda SuccessMessage kaybetmek istemiyoruz bu nedenle TempData i�erisinde tutuyoruz
                TempData["SuccessMessage"] = "Kullan�c� ba�ar�yla olu�turuldu."; 
                return RedirectToAction(nameof(HomeController.SignUp));
            }

            //hata durumunda
            ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());
            return View();
        }
        #endregion

        #region SignIn

        public async Task<IActionResult> SignIn()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel request, string returnUrl=null)
        {
            returnUrl = returnUrl ?? Url.Action("Index", "Home");

            var hasUser =await _UserManager.FindByEmailAsync(request.Email);

            if(hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Email veya �ifre yanl��");
                return View();
            }

            //1. parametre kullan�c� bilgileri 2. parametre �ifre
            //3. parametre hat�rlama s�resi, kullan�c� bilgilerinin oturumu kapatsa dahi belirledi�imiz s�re boyunca cookie de kalmas�n� sa�lar
            //4. parametre ise tekrarl� bi �ekilde hatal� �ifre girilmesi durumunda kullan�c�n�n kilitlenmesi, false olursa kilitlenmez
            var signInResult = await _SignInManager.PasswordSignInAsync(hasUser, request.Password, request.RememberMe, false);

            if (signInResult.Succeeded)
            {
                return Redirect(returnUrl);
            }

            ModelState.AddModelErrorList(new List<string>() { "Email veya �ifre yanl��" });
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
