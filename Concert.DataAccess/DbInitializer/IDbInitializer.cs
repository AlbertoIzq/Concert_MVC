using Concert.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concert.DataAccess.DbInitializer
{
    public interface IDbInitializer
    {
        /// <summary>
        /// Responsible for creating admin user and roles
        /// </summary>
        void Initialize(ApplicationUser adminUser, string passwordAdminUser);
    }
}
