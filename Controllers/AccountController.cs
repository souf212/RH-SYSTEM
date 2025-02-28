using HR_Management_System.Data;
using HR_Management_System.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HR_Management_System.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.Utilisateurs.FirstOrDefault(u => u.Login == model.Username);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Nom d'utilisateur ou mot de passe invalide.");
                return View(model);
            }

            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {

                var hashedInputPassword = Convert.ToBase64String(sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(model.Password)));
                if (user.Password != hashedInputPassword)
                {
                    Console.WriteLine($"Computed Hash: {hashedInputPassword}");
                    Console.WriteLine($"user password: {user.Password}");
                    ModelState.AddModelError(string.Empty, "Nom d'utilisateur ou mot de passe invalide.");
                    return View(model);
                }
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return user.Role switch
            {
                "Admin" => RedirectToAction("Index", "AdminDashboard"),
                "User" => RedirectToAction("Index", "UserDashboard"),
                _ => RedirectToAction("Login", "Account")
            };
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
