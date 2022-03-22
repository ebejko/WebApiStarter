using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApiStarter.Constants;

namespace WebApiStarter.Data
{
    public static class DbInitializer
    {
        public async static Task Seed(
            ApplicationDbContext context, 
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            await context.Database.MigrateAsync();

            if (!context.Roles.Any())
            {
                var role = new IdentityRole(RoleConstants.Admin);
                await roleManager.CreateAsync(role);
            }

            if (!context.Users.Any())
            {
                await userManager.CreateUser("admin", "admin@mail.com", "Pa$$w0rd", RoleConstants.Admin);
                await userManager.CreateUser("user", "user@mail.com", "Pa$$w0rd");
            }
        }

        private static async Task CreateUser(this UserManager<IdentityUser> userManager, string userName, string email, string password, string? role = null)
        {
            var user = new IdentityUser(userName)
            {
                Email = email
            };

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                user.EmailConfirmed = true;
                await userManager.UpdateAsync(user);

                if (!String.IsNullOrWhiteSpace(role))
                    await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}
