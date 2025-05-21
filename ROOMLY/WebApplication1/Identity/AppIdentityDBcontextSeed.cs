using Microsoft.AspNetCore.Identity;
using WebApplication1.models;

namespace ROOMLY.Identity
{
    public class AppIdentityDBcontextSeed
    {
        // Function for seeding users
        public static async Task SeedUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Check if roles exist, if not, create them
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
                await roleManager.CreateAsync(new IdentityRole("User"));
                await roleManager.CreateAsync(new IdentityRole("Receptionist"));
            }

    
            if (!userManager.Users.Any())
            {
                var user = new ApplicationUser
                {
                    UserName = "HagerAymen",
                    PhoneNumber = "01091285207",
                    Email = "hagerayman737@gmail.com",
                    FullName = "Hager Aymen",
                    Address = "Your Address Here"
                };

                // Create the user with a password
                var result = await userManager.CreateAsync(user, "P@ssw0rd123!");

                if (result.Succeeded)
                {
                    // Assign the role to the user
                    await userManager.AddToRoleAsync(user, "Admin");
                }
                else
                {
                    Console.WriteLine("User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                Console.WriteLine("User already exists.");
            }
        }
    }
}
