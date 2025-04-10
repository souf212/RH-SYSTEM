using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PFA_TEMPLATE.Data; // For ApplicationDbContext
using PFA_TEMPLATE.Interfaces;
using PFA_TEMPLATE.Repositories;
using PFA_TEMPLATE.Services;
using PFA_TEMPLATE.ViewModels;
using System.Text.Json.Serialization; 
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PusherServer;
using System.Security.Claims;

namespace PFA_TEMPLATE;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();


        builder.Services.AddControllersWithViews();
        builder.Services.AddScoped<IPlanningRepository, PlanningRepository>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IPasswordHasher, PasswordHasherService>();
        builder.Services.AddScoped<ITacheService, TacheService>();
        builder.Services.AddScoped<IEmailService, GmailEmailService>();
        builder.Services.AddScoped<GenerationEmploiService>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
        builder.Services.AddSignalR();
        builder.Services.AddSingleton<PusherService>();


        // Add this to your services configuration
        builder.Services.AddSingleton<Pusher>(provider => new Pusher(
            builder.Configuration["Pusher:AppId"],
            builder.Configuration["Pusher:AppKey"],
            builder.Configuration["Pusher:AppSecret"],
            new PusherOptions
            {
                Cluster = builder.Configuration["Pusher:Cluster"],
                Encrypted = true
            }
        ));
        // API configuration
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                options.JsonSerializerOptions.WriteIndented = true;
            });
        // Configure Entity Framework and connect to the database
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
       options.UseMySql(
           builder.Configuration.GetConnectionString("DefaultConnection"),
           new MySqlServerVersion(new Version(8, 0, 33)),
           mysqlOptions =>
           {
               mysqlOptions.EnableRetryOnFailure();
           })
       .EnableSensitiveDataLogging()
       .EnableDetailedErrors());
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
            options.AddPolicy("Employes", policy => policy.RequireRole("Employes"));
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
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
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
            // Add a Pusher channel authentication endpoint
            endpoints.MapPost("/pusher/auth", async context =>
            {
                if (!context.User.Identity.IsAuthenticated)
                {
                    context.Response.StatusCode = 401;
                    return;
                }

                // Get current user ID
                var currentUserId = int.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Create a Pusher instance
                var configuration = context.RequestServices.GetRequiredService<IConfiguration>();
                var options = new PusherServer.PusherOptions
                {
                    Cluster = configuration["Pusher:Cluster"],
                    Encrypted = true
                };

                var pusher = new PusherServer.Pusher(
                    configuration["Pusher:AppId"],
                    configuration["Pusher:Key"],
                    configuration["Pusher:Secret"],
                    options
                );

                // Get socketId and channel name from request
                var form = await context.Request.ReadFormAsync();
                var socketId = form["socket_id"].ToString();
                var channelName = form["channel_name"].ToString();

                // Authenticate the channel
                var result = pusher.Authenticate(channelName, socketId);

                // Return authentication response
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result.ToJson());
            });

            app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Account}/{action=Login}/{id?}");

            // Define the default routing pattern

            app.Run();
        }); 
    }
    public static IHostBuilder CreateHostBuilder(string[] args) =>
       Host.CreateDefaultBuilder(args)
           .ConfigureLogging(logging =>
           {
               logging.ClearProviders();
               logging.AddConsole();
               logging.AddDebug();
           })
           .ConfigureWebHostDefaults(webBuilder =>
           {
               webBuilder.UseStartup<Program>();
           });
}