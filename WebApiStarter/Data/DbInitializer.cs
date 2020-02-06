using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
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
                var user = new IdentityUser("ebejko")
                {
                    Email = "ebejko@mail.com"
                };

                var result = await userManager.CreateAsync(user, "Pa$$w0rd");
				
                if (result.Succeeded)
                {
                    user.EmailConfirmed = true;
                    await userManager.UpdateAsync(user);
                    await userManager.AddToRoleAsync(user, RoleConstants.Admin);
                }
            }
        }
    }
}
