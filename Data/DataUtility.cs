using AstraBlog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AstraBlog.Data
{
    public static class DataUtility
    {
        private const string _adminRole = "Admin";
        private const string _moderatorRole = "Moderator";
        private const string _adminEmail = "ryanpenley@mailinator.com";
        private const string _moderatorEmail = "ryanpenleydev@mailinator.com";


        public static DateTime GetPostGresDate(DateTime datetime)
        {
            return DateTime.SpecifyKind(datetime, DateTimeKind.Utc);
        }


        public static string GetConnectionString(IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("DefaultConnection"); // Local Connection string

            string? databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL"); // Remote Connection string

            return string.IsNullOrEmpty(databaseUrl) ? connectionString! : BuildConnectionString(databaseUrl);
        }

        private static string BuildConnectionString(string databaseUrl)
        {
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            var builder = new NpgsqlConnectionStringBuilder()
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };
            return builder.ToString();
        }

        public static async Task ManageDataAsync(IServiceProvider svcProvider)
        {

            // Service: An instance of RoleManager
            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();

            // Service: An instance of Configuration Service
            var configurationSvc = svcProvider.GetRequiredService<IConfiguration>();


            // Service: An instance of UserManager
            var userManagerSvc = svcProvider.GetRequiredService<UserManager<BlogUser>>();


            // Service: An instance of RoleManager
            var roleManagerSvc = svcProvider.GetRequiredService<RoleManager<IdentityRole>>();
            

            // Migration: This is the programmatic equivalent to Update-Database
            await dbContextSvc.Database.MigrateAsync();


            // Seed Roles
            await SeedRolesAsync(roleManagerSvc);

            // Seed Users
            await SeedUsersAsync(dbContextSvc, configurationSvc, userManagerSvc);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(_adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(_adminRole));
            }
            if (!await roleManager.RoleExistsAsync(_moderatorRole))
            {
                await roleManager.CreateAsync(new IdentityRole(_moderatorRole));
            }
        }


        private static async Task SeedUsersAsync(ApplicationDbContext context, IConfiguration configuration, UserManager<BlogUser> userManager)
        {
            // Seed Admin User
            if (!context.Users.Any(u => u.Email == _adminEmail))
            {
                BlogUser adminUser = new()
                {
                    Email = _adminEmail,
                    UserName = _adminEmail,
                    FirstName = "Ryan",
                    LastName = "Penley",
                    EmailConfirmed = true,
                };

                await userManager.CreateAsync(adminUser, configuration["AdminPwd"] ?? Environment.GetEnvironmentVariable("AdminPwd")!);
                await userManager.AddToRoleAsync(adminUser, _adminRole);
            }
            
            // Seed Moderator User
            if (!context.Users.Any(u => u.Email == _moderatorEmail))
            {
                BlogUser moderatorUser = new()
                {
                    Email = _moderatorEmail,
                    UserName = _moderatorEmail,
                    FirstName = "Blog",
                    LastName = "Moderator",
                    EmailConfirmed = true,
                };

                await userManager.CreateAsync(moderatorUser, configuration["ModeratorPwd"] ?? Environment.GetEnvironmentVariable("ModeratorPwd")!);
                await userManager.AddToRoleAsync(moderatorUser, _moderatorRole);
            }
        }
    }
}
