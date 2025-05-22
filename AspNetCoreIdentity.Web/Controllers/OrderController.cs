using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreIdentity.Web.Controllers
{
    public class OrderController : Controller
    {
        [Authorize(Policy = "OrderPermissionReadAndDelete")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PermissionCheck()
        {
            return View();
        }
    }
}
