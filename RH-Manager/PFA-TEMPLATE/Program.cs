using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PFA_TEMPLATE.Data; // For ApplicationDbContext
using PFA_TEMPLATE.Repositories;
using PFA_TEMPLATE.Services;
using System.Text.Json.Serialization;
namespace PFA_TEMPLATE;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();

        // Add services to the container
        builder.Services.AddControllersWithViews();
        builder.Services.AddScoped<IPlanningRepository, PlanningRepository>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ITacheService, TacheService>();
        builder.Services.AddScoped<GenerationEmploiService>();
        builder.Services.AddHttpContextAccessor();
        // API configuration
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                options.JsonSerializerOptions.WriteIndented = true;
            });
        // Configure Entity Framework and connect to the database
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                new MySqlServerVersion(new Version(8, 0, 33))).EnableSensitiveDataLogging());


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
        // Add CORS for API access from other applications
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder => builder.AllowAnyOrigin()
                                  .AllowAnyMethod()
                                  .AllowAnyHeader());
        });

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        //app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        // Middleware for authentication and session management
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSession();
        // Enable MVC routes and API controllers
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers(); // Activate API routes
            endpoints.MapDefaultControllerRoute(); // Activate MVC routes

            foreach (var endpoint in endpoints.DataSources.SelectMany(ds => ds.Endpoints))
            {
                Console.WriteLine($" Route Active: {endpoint.DisplayName}");
            }
        });

        // Define the default routing pattern

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Account}/{action=Login}/{id?}");

        app.Run();
    }
}
