using HR_Management_System.Data; // For ApplicationDbContext
using HR_Management_System.Repositories;
using HR_Management_System.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
namespace HR_Management_System;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        builder.Services.AddControllersWithViews();
        builder.Services.AddScoped<IPlanningRepository, PlanningRepository>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ITacheService, TacheService>();
        builder.Services.AddScoped<GenerationEmploiService>();

        // Configure Entity Framework and connect to the database
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                new MySqlServerVersion(new Version(8, 0, 33))));

        // Configure authentication using cookies
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login"; // Redirect to login page
                options.LogoutPath = "/Account/Logout"; // Redirect to logout action
                options.AccessDeniedPath = "/Account/AccessDenied"; // Redirect for unauthorized access
                options.SlidingExpiration = true;
            });
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
            options.AddPolicy("User", policy => policy.RequireRole("User"));
        });

        builder.Services.AddSession();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        // Middleware for authentication and session management
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSession();

        // Define the default routing pattern

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Account}/{action=Login}/{id?}");

        app.Run();
    }
}
