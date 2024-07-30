using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MvcBul.DataAccess.Data;
using MvcBul.Models;
using MvcBul.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcBul.DataAccess.DBInitialzer
{
    public class DBInitializer : IDBInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public DBInitializer(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        public void initialize()
        {
            try
            {
                if(_context.Database.GetPendingMigrations().Count() > 0)
                {
                    _context.Database.Migrate();
                }

                if (!_roleManager.RoleExistsAsync(SD.Role_User_Cust).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Cust)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Comp)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Admin)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Employee)).GetAwaiter().GetResult();

                    var user = _userManager.CreateAsync(new ApplicationUser
                    {
                        UserName = "admin@milano.to",
                        Email = "admin@milano.to",
                        EmailConfirmed= true,
                        Name = "Admin",
                        PhoneNumber = "424242424242",
                        State = "ON",
                        StreetAddress = "Address 1 ",
                        City = "Etobicoke",
                        PostalCode = "M9B4R1"
                    }, "Admin123*").GetAwaiter().GetResult();

                    var userFromDB = _context.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@milano.to");

                    _userManager.AddToRoleAsync(userFromDB, SD.Role_User_Admin).GetAwaiter().GetResult();

                }
                var userFromDB1 = _context.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@milano.to");
         
                return;

            }
            catch (Exception ex)
            {

            }
        }
    }
}
