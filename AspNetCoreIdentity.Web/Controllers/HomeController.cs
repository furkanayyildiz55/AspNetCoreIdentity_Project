using AspNetCoreIdentity.Web.Extensions;
using AspNetCoreIdentity.Web.Models;
using AspNetCoreIdentity.Web.Services;
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
        private readonly SignInManager<AppUser> _SignInManager;
        private readonly IEmailService _EmailService;

        public HomeController(ILogger<HomeController> logger , UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
        {
            _logger = logger;
            _UserManager = userManager;
            _SignInManager = signInManager;
            _EmailService = emailService;
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
                ModelState.AddModelError(string.Empty, "Email veya þifre yanlýþ");
                return View();
            }

            //1. parametre kullanýcý bilgileri 2. parametre þifre
            //3. parametre hatýrlama süresi, kullanýcý bilgilerinin oturumu kapatsa dahi belirlediðimiz süre boyunca cookie de kalmasýný saðlar
            //4. parametre ise tekrarlý bi þekilde hatalý þifre girilmesi durumunda kullanýcýnýn kilitlenmesi, false olursa kilitlenmez
            var signInResult = await _SignInManager.PasswordSignInAsync(hasUser, request.Password, request.RememberMe, true);

            if (signInResult.Succeeded)
            {
                return Redirect(returnUrl);
            }

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelErrorList(new List<string>() { "Kullanýcý kilitlendi. 3 dakika boyunca giriþ yapýlamaz." });
                return View();
            }

            int failedCount = await _UserManager.GetAccessFailedCountAsync(hasUser);
            ModelState.AddModelErrorList(new List<string>() { $"Email veya þifre yanlýþ. Baþarýsýz deneme sayýsý {failedCount}" });
            return View();
        }

        #endregion

        #region ForgetPassword ResetPassword

        public async Task<IActionResult> ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel request)
        {
            var hasUser = await _UserManager.FindByEmailAsync(request.Email);

            if(hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Email adresi bulunamadý.");
                return View();
            }

            string passwordResetToken = await _UserManager.GeneratePasswordResetTokenAsync(hasUser);
            var passwordResetLink = Url.Action("ResetPassword", "Home", new { userId = hasUser.Id, token = passwordResetToken }, HttpContext.Request.Scheme);

            await _EmailService.SendResetPasswordEmail(passwordResetLink, request.Email);

            TempData["SuccessMessage"] = "Þifre sýfýrlama linki email adresinize gönderildi.";

            return RedirectToAction(nameof(ForgetPassword));
        }

        //Framework queryStringdeki veriler metot parametrelerini otomatik olarak doldurur
        public async Task<IActionResult> ResetPassword(string userId, string token)
        {
            //TempData ile queryStringdeki post metoduna gönderiyoruz.  
            //Bunu yapmanýn bir diðer yöntemi ise cshtml de verileri hidden input ile saklayýp post metoduna göndermektir.
            TempData["userId"] = userId;
            TempData["token"] = token;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel request)
        {
            var  userID = TempData["userId"];
            var token = TempData["token"];

            if(userID == null || token == null)
            {
                throw new Exception("Bir hata oluþtu.");
            }

            var hasUser = await _UserManager.FindByIdAsync(userID.ToString());

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Kullanýcý bulunamadý.");
                return View();
            }

            var result = await _UserManager.ResetPasswordAsync(hasUser,token.ToString(), request.Password);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Þifre baþarýyla sýfýrlandý.";
            }
            else
            {
                ModelState.AddModelErrorList(result.Errors.Select(x => x.Description).ToList());
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
