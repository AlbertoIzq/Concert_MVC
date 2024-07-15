using Concert.DataAccess.Data;
using Concert.DataAccess.Repository.IRepository;
using Concert.Models;
using Concert.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concert.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        public void Initialize(ApplicationUser adminUser, string passwordAdminUser)
        {
            // Push all pending migrations if they are not applied
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex) { }

            // Create roles if they are not created
            if (!_roleManager.RoleExistsAsync(SD.ROLE_ADMIN).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.ROLE_CUSTOMER)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.ROLE_COMPANY)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.ROLE_ADMIN)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.ROLE_EMPLOYEE)).GetAwaiter().GetResult();

                // If roles are not created, then we will create admin user as well
                _userManager.CreateAsync(adminUser, passwordAdminUser).GetAwaiter().GetResult();

                ApplicationUser user = _db.ApplicationUser.FirstOrDefault(u => u.Email == "testadmin@gmail.com");
                _userManager.AddToRoleAsync(user, SD.ROLE_ADMIN).GetAwaiter().GetResult();
            }

            return;
        }
    }
}
