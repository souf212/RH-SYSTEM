using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PFA_TEMPLATE.Data;
using PFA_TEMPLATE.ViewModels;
using PFA_TEMPLATE.Services;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Vonage;
using Vonage.Request;
using Vonage.Messaging;
using System.Text;
namespace PFA_TEMPLATE.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public AccountController(
            ApplicationDbContext context,
            IEmailService emailService)
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
                // Security best practice: Don't reveal whether username exists
                ModelState.AddModelError(string.Empty, "Invalid username or password");
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

            // Find user by email
            var user = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower());

            if (user == null)
            {
                // Email n'existe pas - informer l'utilisateur
                ModelState.AddModelError(nameof(model.Email), "Aucun compte associé à cet email n'a été trouvé.");
                return View(model);
            }

            // Generate a password reset token
            string token = GeneratePasswordResetToken(user.Id);

            // Create reset password link
            string resetLink = Url.Action(
                "ResetPassword",
                "Account",
                new { token = token },
                Request.Scheme
            );

            // Send email with reset link
            string emailBody = $@"
            Bonjour {user.Prenom} {user.Nom},

            Vous avez demandé une réinitialisation de mot de passe. 
            Cliquez sur le lien ci-dessous pour réinitialiser votre mot de passe :

            {resetLink}

            Si vous n'avez pas demandé cette réinitialisation, ignorez simplement cet email.

            Cordialement,
            Votre Équipe";

            try
            {
                await _emailService.SendEmailAsync(
                    user.NomComplet,
                    user.Email,
                    "Réinitialisation de mot de passe",
                    emailBody
                );

                // Inform user that email has been sent
                TempData["SuccessMessage"] = "Un lien de réinitialisation a été envoyé à votre email.";
                return RedirectToAction("Login");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Une erreur est survenue lors de l'envoi de l'email.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }

            var resetPasswordModel = new ResetPasswordViewModel
            {
                Token = token
            };

            return View(resetPasswordModel);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Validate and decode the token
            int userId = ValidatePasswordResetToken(model.Token);

            if (userId == 0)
            {
                ModelState.AddModelError(string.Empty, "Le lien de réinitialisation est invalide ou a expiré.");
                return View(model);
            }

            // Find the user
            var user = await _context.Utilisateurs.FindAsync(userId);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Utilisateur non trouvé.");
                return View(model);
            }

            // Hash the new password
            string hashedPassword = HasherProgram.HashPassword(model.NewPassword);

            // Update the password
            user.Password = hashedPassword;
            _context.Utilisateurs.Update(user);
            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] = "Votre mot de passe a été réinitialisé avec succès.";
            return RedirectToAction("Login");
        }

        // Helper method to generate a secure password reset token
        private string GeneratePasswordResetToken(int userId)
        {
            // Combine user ID with a timestamp and a random salt
            var timestamp = DateTime.UtcNow.Ticks;
            var salt = Guid.NewGuid().ToString();
            var tokenData = $"{userId}|{timestamp}|{salt}";

            // Base64 encode the token for safe transmission
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(tokenData))
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }

        // Helper method to validate and decode the password reset token
        private int ValidatePasswordResetToken(string token)
        {
            try
            {
                // Decode the token
                var decodedToken = Encoding.UTF8.GetString(
                    Convert.FromBase64String(
                        token.Replace("-", "+")
                             .Replace("_", "/")
                             .PadRight(token.Length + (4 - token.Length % 4) % 4, '=')
                    )
                );

                // Split the token
                var parts = decodedToken.Split('|');
                if (parts.Length != 3)
                    return 0;

                // Parse user ID and timestamp
                if (!int.TryParse(parts[0], out int userId))
                    return 0;

                var tokenTime = new DateTime(long.Parse(parts[1]));

                // Check token expiration (valid for 1 hour)
                if (DateTime.UtcNow - tokenTime > TimeSpan.FromHours(1))
                    return 0;

                return userId;
            }
            catch
            {
                return 0;
            }
        }

    }

}

