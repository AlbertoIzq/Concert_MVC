using Concert.DataAccess.Data;
using Concert.DataAccess.Repository;
using Concert.DataAccess.Repository.IRepository;
using Concert.Models;
using Concert.Models.ViewModels;
using Concert.Utility;
using DotEnv.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ConcertWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ROLE_ADMIN)]
    public class UserController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(IUnitOfWork unitOfWork,
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagement(string userId)
        {
            RoleManagementVM roleManagementVM = new()
            {
                ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId, includeProperties: "Company"),
                RoleList = _roleManager.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = _unitOfWork.Company.GetAll().OrderBy(u => u.Name).Select(u => new SelectListItem()
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                
            };

            roleManagementVM.ApplicationUser.Role = _userManager.GetRolesAsync(roleManagementVM.ApplicationUser).
                GetAwaiter().GetResult().FirstOrDefault();

            return View(roleManagementVM);
        }

        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM roleManagementVM)
        {

            string oldRole = _userManager.GetRolesAsync(roleManagementVM.ApplicationUser).
                GetAwaiter().GetResult().FirstOrDefault();

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == roleManagementVM.ApplicationUser.Id, includeProperties: "Company");

            if (roleManagementVM.ApplicationUser.Role != oldRole)
            {
                // A Role was updated
                if (roleManagementVM.ApplicationUser.Role == SD.ROLE_COMPANY)
                {
                    applicationUser.CompanyId = roleManagementVM.ApplicationUser.CompanyId;
                }
                if (oldRole == SD.ROLE_COMPANY)
                {
                    applicationUser.CompanyId = null;
                }
                _unitOfWork.ApplicationUser.Update(applicationUser);
                _unitOfWork.Save();

                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, roleManagementVM.ApplicationUser.Role).GetAwaiter().GetResult();
            }
            else
            {
                if (oldRole == SD.ROLE_COMPANY && applicationUser.CompanyId != roleManagementVM.ApplicationUser.CompanyId)
                {
                    applicationUser.CompanyId = roleManagementVM.ApplicationUser.CompanyId;
                    _unitOfWork.ApplicationUser.Update(applicationUser);
                    _unitOfWork.Save();
                }
            }
            return RedirectToAction("Index");
        }

        #region ApiCalls

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> userList = _unitOfWork.ApplicationUser.GetAll(includeProperties: "Company").ToList();

            foreach (var user in userList)
            { 
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

                if (user.Company == null)
                {
                    user.Company = new Company() { Name = "" };
                }
            }

            return Json(new { data = userList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            var userFromDb = _unitOfWork.ApplicationUser.Get(u => u.Id == id);
            if (userFromDb == null || userFromDb.Email == GetUserAdminEmail())
            {
                return Json(new { success = false, message = "Error while locking/unlocking" });
            }

            string operation = string.Empty;

            // User is currently locked and we need to unlock them
            if (userFromDb.LockoutEnd != null && userFromDb.LockoutEnd > DateTime.Now)
            {
                userFromDb.LockoutEnd = DateTime.Now;
                operation = "unlocked";
            }
            else
            {
                userFromDb.LockoutEnd = DateTime.Now.AddYears(SD.USER_LOCK_YEARS);
                operation = "Locked";
            }

            _unitOfWork.ApplicationUser.Update(userFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = $"User {operation} successfully" });
        }

        #endregion

        private string GetUserAdminEmail()
        {
            string userAdminEmail = string.Empty;

            string envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            new EnvLoader().Load();
            var envVarReader = new EnvReader();

            if (envName == SD.ENVIRONMENT_DEVELOPMENT)
            {
                userAdminEmail = envVarReader["AdminUser_Email"];
            }
            else if (envName == SD.ENVIRONMENT_PRODUCTION)
            {
                userAdminEmail = Environment.GetEnvironmentVariable("AdminUser_Email");
            }

            return userAdminEmail;
        }   
    }
}