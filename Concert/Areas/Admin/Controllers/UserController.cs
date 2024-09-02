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
        private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(ApplicationDbContext db,
            IUnitOfWork unitOfWork,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagement(string userId)
        {
            string roleId = _db.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId;

            RoleManagementVM roleManagementVM = new()
            {
                // The same: ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId, includeProperties: "Company")
                ApplicationUser = _db.ApplicationUser.Include(u => u.Company).FirstOrDefault(u => u.Id == userId),
                RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                }),
                CompanyList = _unitOfWork.Company.GetAll().OrderBy(u => u.Name).Select(u => new SelectListItem()
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                
            };

            // The same: roleManagementVM.ApplicationUser.Role = _userManager.GetRolesAsync(roleManagementVM.ApplicationUser).GetAwaiter().GetResult().FirstOrDefault();
            roleManagementVM.ApplicationUser.Role = _db.Roles.FirstOrDefault(u => u.Id == roleId).Name;

            return View(roleManagementVM);
        }

        #region ApiCalls

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> userList = _db.ApplicationUser.Include(u => u.Company).ToList();

            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            foreach (var user in userList)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;

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
            var userFromDb = _db.ApplicationUser.FirstOrDefault(u => u.Id == id);
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

            // This will work because the record is being tracked by EFC
            _db.SaveChanges();

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