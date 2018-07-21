
using System;
using System.Linq;
using System.Threading.Tasks;
using FiberMapWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FiberMapWeb.Data
{
    public static class SeedData
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            // Look for any users.
            if (context.Users.Any())
            {
                return; // DB has been seeded
            }

            await CreateDefaultUserAndRoleForApplication(userManager, roleManager);
        }

        private static async Task CreateDefaultUserAndRoleForApplication(UserManager<ApplicationUser> um, RoleManager<IdentityRole> rm)
        {
            const string administratorRole = "Administrator";
            const string email = "filter@codedragon.com";

            await CreateDefaultAdministratorRole(rm, administratorRole);
            var user = await CreateDefaultUser(um, email);
            await SetPasswordForDefaultUser(um, email, user);
            await AddDefaultRoleToDefaultUser(um, email, administratorRole, user);
        }

        private static async Task CreateDefaultAdministratorRole(RoleManager<IdentityRole> rm, string administratorRole)
        {
            System.Diagnostics.Debug.WriteLine($"Create the role `{administratorRole}` for application");
            var ir = await rm.CreateAsync(new IdentityRole(administratorRole));
            if (ir.Succeeded)
            {
                System.Diagnostics.Debug.WriteLine($"Created the role `{administratorRole}` successfully");
            }
            else
            {
                var exception = new ApplicationException($"Default role `{administratorRole}` cannot be created");
                System.Diagnostics.Debug.WriteLine(exception, GetIdentiryErrorsInCommaSeperatedList(ir));
                throw exception;
            }
        }

        private static async Task<ApplicationUser> CreateDefaultUser(UserManager<ApplicationUser> um, string email)
        {
            System.Diagnostics.Debug.WriteLine($"Create default user with email `{email}` for application");

            var user = new ApplicationUser
            {
                Email = email,
                NormalizedEmail = email.ToUpper(),
                UserName = email,
                NormalizedUserName = email.ToUpper(),
                PhoneNumber = "",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            var ir = await um.CreateAsync(user);
            if (ir.Succeeded)
            {
                System.Diagnostics.Debug.WriteLine($"Created default user `{email}` successfully");
            }
            else
            {
                var exception = new ApplicationException($"Default user `{email}` cannot be created");
                System.Diagnostics.Debug.WriteLine(exception, GetIdentiryErrorsInCommaSeperatedList(ir));
                throw exception;
            }

            var createdUser = await um.FindByEmailAsync(email);
            return createdUser;
        }

        private static async Task SetPasswordForDefaultUser(UserManager<ApplicationUser> um, string email, ApplicationUser user)
        {
            System.Diagnostics.Debug.WriteLine($"Set password for default user `{email}`");
            const string password = "Password1!";
            var ir = await um.AddPasswordAsync(user, password);
            if (ir.Succeeded)
            {
                System.Diagnostics.Debug.WriteLine($"Set password `{password}` for default user `{email}` successfully");
            }
            else
            {
                var exception = new ApplicationException($"Password for the user `{email}` cannot be set");
                System.Diagnostics.Debug.WriteLine(exception, GetIdentiryErrorsInCommaSeperatedList(ir));
                throw exception;
            }
        }

        private static async Task AddDefaultRoleToDefaultUser(UserManager<ApplicationUser> um, string email, string administratorRole, ApplicationUser user)
        {
            System.Diagnostics.Debug.WriteLine($"Add default user `{email}` to role '{administratorRole}'");
            var ir = await um.AddToRoleAsync(user, administratorRole);
            if (ir.Succeeded)
            {
                System.Diagnostics.Debug.WriteLine($"Added the role '{administratorRole}' to default user `{email}` successfully");
            }
            else
            {
                var exception = new ApplicationException($"The role `{administratorRole}` cannot be set for the user `{email}`");
                System.Diagnostics.Debug.WriteLine(exception, GetIdentiryErrorsInCommaSeperatedList(ir));
                throw exception;
            }
        }

        private static string GetIdentiryErrorsInCommaSeperatedList(IdentityResult ir)
        {
            string errors = null;
            foreach (var identityError in ir.Errors)
            {
                errors += identityError.Description;
                errors += ", ";
            }
            return errors;
        }
    }
}