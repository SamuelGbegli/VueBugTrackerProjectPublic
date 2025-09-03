using VueBugTrackerProject.Classes;

using Sodium;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Diagnostics;

namespace VueBugTrackerProject.Server
{
    /// <summary>
    /// Static class to generate data in the application.
    /// </summary>
    public static class SeedData
    {
        public static async Task SeedAll(DatabaseContext context, UserManager<Account> userManager, RoleManager<IdentityRole> roleManager)
        {
            await SeedRoles(roleManager);
            await SeedSuperUser(context, userManager);
        }
        /// <summary>
        /// Adds a super user account to the app's database, if one doesn't already
        /// exist.
        /// </summary>
        /// <param name="context">The database context.</param>
        public static async Task SeedSuperUser(DatabaseContext context, UserManager<Account> userManager)
        {
            //Exits function if context is null
            if(context == null) return;

            //Exits function if there is a super user
            if ((await userManager.GetUsersInRoleAsync("Super user")).Any()) return;

            //Creates super user account
            var superUser = new Account
            {
                UserName = "Super User",
                Email = "SuperUser@VueBugTracker.com",
                Role = AccountRole.SuperUser,
                DateCreated = DateTime.Now
            };

            await userManager.AddToRoleAsync(superUser, "Super user");

            //Adds and saves user
            var result = await userManager.CreateAsync(superUser, "TestPassword1");

        }

        /// <summary>
        /// Adds token stores to existing users that do not have one.
        /// </summary>
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (roleManager == null) return;
            if(roleManager.Roles.Any()) return;
            await roleManager.CreateAsync(new IdentityRole("Normal"));
            await roleManager.CreateAsync(new IdentityRole("Administrator"));
            await roleManager.CreateAsync(new IdentityRole("Super user"));

        }
    }
}
