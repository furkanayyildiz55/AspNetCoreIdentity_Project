using AspNetCoreIdentity.Web.Areas.Admin.Models;
using AspNetCoreIdentity.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentity.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public partial class HomeController : Controller
    {
        private readonly UserManager<AppUser> _UserManager;

        public HomeController(UserManager<AppUser> userManager)
        {
            _UserManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UserList()
        {
            var userList = await _UserManager.Users.ToListAsync();

            var userViewModelList = userList.Select(x => new UserViewModel() {
                Email = x.Email,
                Name = x.UserName,
                Id = x.Id,
            }).ToList();

            return View(userViewModelList);
        }

    }
}
