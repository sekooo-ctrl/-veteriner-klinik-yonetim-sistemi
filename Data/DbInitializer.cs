
using Microsoft.AspNetCore.Identity;

namespace VeterinerKlinik.Web.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            var roleManager = serviceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            var userManager = serviceProvider
                .GetRequiredService<UserManager<IdentityUser>>();

            string[] roller = { "Admin", "Veteriner", "Uye" };

            foreach (var rol in roller)
            {
                if (!await roleManager.RoleExistsAsync(rol))
                {
                    var sonuc = await roleManager.CreateAsync(
                        new IdentityRole(rol));

                    if (!sonuc.Succeeded)
                        throw new Exception($"'{rol}' rolü oluşturulamadı.");
                }
            }

            var adminEmail = configuration["SeedAdmin:Email"];
            var adminPassword = configuration["SeedAdmin:Password"];

            if (string.IsNullOrWhiteSpace(adminEmail) ||
                string.IsNullOrWhiteSpace(adminPassword))
            {
                return;
            }

            var admin = await userManager.FindByEmailAsync(adminEmail);

            if (admin == null)
            {
                admin = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var sonuc = await userManager.CreateAsync(
                    admin, adminPassword);

                if (!sonuc.Succeeded)
                {
                    var hatalar = string.Join(", ",
                        sonuc.Errors.Select(e => e.Description));

                    throw new Exception(hatalar);
                }
            }

            if (!await userManager.IsInRoleAsync(admin, "Admin"))
            {
                var sonuc = await userManager.AddToRoleAsync(
                    admin, "Admin");

                if (!sonuc.Succeeded)
                    throw new Exception("Admin rolü atanamadı.");
            }
        }
    }
}
