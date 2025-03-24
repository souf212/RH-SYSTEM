using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PFA_TEMPLATE.Data;
using PFA_TEMPLATE.ViewModels;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using PFA_TEMPLATE.Interfaces;

namespace PFA_TEMPLATE.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        public AccountController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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

            // Query the database using the Login field
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
        new Claim(ClaimTypes.Role, user.Role),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) // Add the user's ID as NameIdentifier
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Redirection basée sur le rôle
            return user.Role switch
            {
                "Admin" => RedirectToAction("Index", "AdminDashboard"),
                "Employes" => RedirectToAction("Index", "UserDashboard"),
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
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Retrieve the user from the database
            var user = _context.Utilisateurs.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "No user found with this email address.");
                return View(model);
            }

            try
            {
                // Prepare the email content
                var subject = "Your Password";
                var body = $"Your password is: {user.Password}"; // Send the password (not recommended for production)

                // Send the email to the user's email address
                await _emailService.SendEmailAsync(user.Email, subject, body);

                ViewBag.Message = "Your password has been sent to your email address.";
                return View();
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"Error sending email: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                ModelState.AddModelError(string.Empty, "An error occurred while sending the email. Please try again later.");
                return View(model);
            }
        }
    }
}
