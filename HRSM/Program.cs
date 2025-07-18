using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using HRSM.Data;
using HRSM.Controllers;

namespace HRSM
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Ensure the connection string exists to prevent null reference errors
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is missing in appsettings.json.");
            }

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Configure MySQL
            builder.Services.AddDbContext<HRSMDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // Register AdminController as a scoped service for activity logging
            builder.Services.AddScoped<AdminController>();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";  // Redirect to login if not authenticated
                    options.LogoutPath = "/Account/Logout"; // Logout path
                    options.AccessDeniedPath = "/Account/AccessDenied"; // Optional: Redirect for unauthorized access
                });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Ensure database is created
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<HRSMDbContext>();
                    await context.EnsureDatabaseCreated();
                    await context.EnsureFeedbackTableExists();
                    await context.AddFeedbackResponseColumnsAsync();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while creating the database.");
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();

            // Add staff department middleware
            app.UseStaffDepartmentRedirection();

            // Area route must come first
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
            );

            // RoomManagement route
            app.MapControllerRoute(
                name: "roomManagement",
                pattern: "Admin/RoomManagement/{action=Index}/{id?}",
                defaults: new { controller = "RoomManagement" }
            );

            // Default route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            // Home route
            app.MapControllerRoute(
                name: "home",
                pattern: "{controller=Home}/{action=Home}/{id?}"
            );

            // Admin route
            app.MapControllerRoute(
                name: "admin",
                pattern: "Admin/{action=AdminDashboard}/{id?}",
                defaults: new { controller = "Admin" }
            );

            await app.RunAsync();
        }
    }
}