using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PFA_TEMPLATE.Data;
using PFA_TEMPLATE.ViewModels;
using System.Security.Claims;

namespace PFA_TEMPLATE.Controllers
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
                Console.WriteLine("❌ Formulaire invalide.");
                return View(model);
            }

            // Vérification insensible à la casse
            var user = _context.Utilisateurs
                .FirstOrDefault(u => u.Login.ToLower() == model.Username.ToLower());

            if (user == null)
            {
                Console.WriteLine($"❌ Utilisateur '{model.Username}' non trouvé.");
                ModelState.AddModelError(string.Empty, "Nom d'utilisateur ou mot de passe invalide.");
                return View(model);
            }

            // Hachage du mot de passe entré
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedInputPassword = Convert.ToBase64String(
                    sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(model.Password))
                );

                Console.WriteLine($"🔍 Hashed input password: {hashedInputPassword}");
                Console.WriteLine($"🔍 Stored password in DB: {user.Password}");

                if (user.Password.Trim() != hashedInputPassword)
                {
                    Console.WriteLine("❌ Mot de passe incorrect.");
                    ModelState.AddModelError(string.Empty, "Nom d'utilisateur ou mot de passe invalide.");
                    return View(model);
                }
            }

            Console.WriteLine($"✅ Connexion réussie pour : {user.Login} ({user.Role})");

            // Création des claims pour l'authentification
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Redirection basée sur le rôle
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
