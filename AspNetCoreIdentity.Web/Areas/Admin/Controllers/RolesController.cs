﻿using AspNetCoreIdentity.Web.Areas.Admin.Models;
using AspNetCoreIdentity.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreIdentity.Web.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreIdentity.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RolesController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public RolesController(RoleManager<AppRole> roleManager , UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        #region Index

        [Authorize(Roles = "Admin,Yönetici")]
        public async  Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles
                .Select(x => new RoleViewModel()
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();

            return View(roles);
        }

        #endregion

        #region RoleCreate

        [Authorize(Roles="Admin,Yönetici")]
        public IActionResult RoleCreate ()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleCreateViewModel request)
        {
            var result = await _roleManager.CreateAsync(new AppRole() 
            {
                Name = request.Name,
            });

            if (!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors);
                return View();
            }

            TempData["SuccessMessage"] = "Rol Oluşturudu.";
            return RedirectToAction(nameof(RolesController.Index));
        }

        #endregion

        #region RoleUpdate

        [Authorize(Roles = "Admin,Yönetici")]
        public async Task<IActionResult> RoleUpdate(string id)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(id);

            if(roleToUpdate == null)
                throw new Exception("Güncellenecek rol bulunamamıştır.");


            return View(new RoleUpdateViewModel() { Id = roleToUpdate.Id, Name=roleToUpdate.Name} );
        }

        [HttpPost]
        public async Task<IActionResult> RoleUpdate(RoleUpdateViewModel request)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(request.Id);
            if (roleToUpdate == null)
            {
                throw new Exception("Güncellenecek rol bulunamamıştır.");
            }

            roleToUpdate.Name = request.Name;
            await _roleManager.UpdateAsync(roleToUpdate);

            ViewData["SuccessMessage"] = "Rol bilgisi güncellenmiştir";

            return View();
        }

        #endregion

        #region RoleDelete
        [Authorize(Roles = "Admin,Yönetici")]
        public async Task<IActionResult> RoleDelete(string id)
        {
            var roleToDelete = await _roleManager.FindByIdAsync(id);

            if (roleToDelete == null)
                throw new Exception("Silinecek rol bulunamamıştır.");

            var result = await _roleManager.DeleteAsync(roleToDelete);

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.Select(x => x.Description).First());
            }


            TempData["SuccessMessage"] = "Rol silinmiştir.";
            return RedirectToAction(nameof(RolesController.Index));
        }

        #endregion

        #region RoleAssign

        public async Task<IActionResult> AssignRoleToUser(string id)
        {
            var currentUser = await _userManager.FindByIdAsync(id);
            ViewBag.userId = id;
            if (currentUser == null)
                throw new Exception("Kullanıcı bulunamamıştır.");

            var roles = await _roleManager.Roles.ToListAsync();

            var rolesViewmodelList = new List<AssignRoleToUserViewModel>();

            var userRoles = await _userManager.GetRolesAsync(currentUser);

            foreach (var role in roles)
            {
                var assignRoleToUserViewModel = new AssignRoleToUserViewModel() { Id = role.Id, Name = role.Name };

                if (userRoles.Contains(role.Name))
                {
                    assignRoleToUserViewModel.Exist = true;
                }
                rolesViewmodelList.Add(assignRoleToUserViewModel);
            }

            return View(rolesViewmodelList);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRoleToUser(string userId, List<AssignRoleToUserViewModel> requestList)
        {
            var userToAssignRoles = await _userManager.FindByIdAsync(userId);

            if (userToAssignRoles == null)
                throw new Exception("Kullanıcı bulunamamıştır.");

            foreach (var role in requestList)
            {

                if (role.Exist)
                {
                    await _userManager.AddToRoleAsync(userToAssignRoles, role.Name);
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(userToAssignRoles, role.Name);
                }
            }

            return RedirectToAction(nameof(HomeController.UserList), "Home");
        }
        #endregion
    }
}
