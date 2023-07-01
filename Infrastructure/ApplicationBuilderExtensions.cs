namespace NBUniforms.Infrastructure
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NBUniforms.Data;
    using NBUniforms.Data.Models;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    using static WebConstants;

    public static class ApplicationBuilderExtensions  //should be static class and method
    {
        //static method to be called on IApplicationBuilder:  app.PrepareDatabase();
        public static IApplicationBuilder PrepareDatabase(this IApplicationBuilder app) //extension method
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var services = serviceScope.ServiceProvider;

            MigrateDatabase(services);

            SeedCategories(services);
            SeedAdministrator(services);

            return app;
        }

        private static void MigrateDatabase(IServiceProvider services)
        {
            //GetRequiredService throws Exception if not found, GetService returns null
            var data = services.GetRequiredService<NBUniformsDbContext>();

            data.Database.Migrate();
        }

        private static void SeedCategories(IServiceProvider services)
        {
            var data = services.GetRequiredService<NBUniformsDbContext>();

            if (data.Categories.Any())
            {
                return;
            }

            data.Categories.AddRange(new[]
            {
                new Category { Name = "Bags" },
                new Category { Name = "Shirts" },
                new Category { Name = "Headwear" },
                new Category { Name = "Jackets" },
                new Category { Name = "Hoodies" },
            });

            data.SaveChanges();
        }

        private static void SeedAdministrator(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            Task
                .Run(async () =>
                {
                    if (await roleManager.RoleExistsAsync(AdministratorRoleName))
                    {
                        return;
                    }

                    var role = new IdentityRole { Name = AdministratorRoleName };

                    await roleManager.CreateAsync(role);

                    const string adminEmail = "admin@nbuniforms.com";
                    const string adminPassword = "admin1";

                    var user = new User
                    {
                        Email = adminEmail,
                        UserName = adminEmail,
                        FullName = "Admin",
                        Cart = new Cart()   //every User has Cart (both have same Id's)
                    };

                    await userManager.CreateAsync(user, adminPassword);

                    await userManager.AddToRoleAsync(user, role.Name);
                })
                .GetAwaiter()
                .GetResult();
        }
    }
}
