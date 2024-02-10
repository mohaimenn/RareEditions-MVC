using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RareEditions.DataAccess.Data;
using RareEditions.Models;
using RareEditions.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RareEditions.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }

        public void Initialize()
        {


            //migraties als ze niet toegepast zijn
            try
            {
                if(_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }catch (Exception ex)
            {

            }

            //maak rollen aan als ze niet aangemaakt zijn

            if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();

                //als rollen niet aangemaakt zijn, dan zullen we ook een admin gebruiker aanmaken.

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@mohaimen.dev",
                    Email = "admin@mohaimen.dev",
                    Name = "Mohaimen aljanabi",
                    PhoneNumber = "003222222",
                    StreetAddress = "klinkkouter",
                    Province = "Oost-vlanderen",
                    PostalCode = "9040",
                    City = "Gent"
                }, "Admin@1234").GetAwaiter().GetResult();

                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@mohaimen.dev");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
            }

            return;
        }
    }
}
