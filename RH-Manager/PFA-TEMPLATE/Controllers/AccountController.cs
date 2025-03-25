using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PFA_TEMPLATE.Data;
using PFA_TEMPLATE.ViewModels;
using PFA_TEMPLATE.Services;
using System.Security.Claims;
using PFA_TEMPLATE.Interfaces;
using Microsoft.EntityFrameworkCore;
using Twilio; 
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account; 
namespace PFA_TEMPLATE.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _twilioAccountSid;
        private readonly string _twilioAuthToken;
        private readonly string _twilioPhoneNumber;
        public AccountController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _twilioAccountSid = configuration["Twilio:AccountSid"];
            _twilioAuthToken = configuration["Twilio:AuthToken"];
            _twilioPhoneNumber = configuration["Twilio:PhoneNumber"];
            TwilioClient.Init(_twilioAccountSid, _twilioAuthToken);
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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string telephone)
        {
            // Find user by telephone number
            var user = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.Telephone == telephone);

            if (user == null)
            {
                ModelState.AddModelError("", "Aucun utilisateur trouvé avec ce numéro de téléphone.");
                return View();
            }

            // Generate a new random password
            string newPassword = GenerateRandomPassword();

            // Hash the new password
            string hashedNewPassword = HasherProgram.HashPassword(newPassword);
            // Update user's password in database
            user.Password = hashedNewPassword;
            await _context.SaveChangesAsync();

            // Send SMS with new password
            try
            {
                SendPasswordViaSMS(user.Telephone, newPassword);
                return View("PasswordResetConfirmation");
            }
            catch (Exception ex)
            {
                // Log the exception
                ModelState.AddModelError("", "Erreur lors de l'envoi du SMS. Veuillez réessayer.");
                return View();
            }
        }

        private string GenerateRandomPassword(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

     

        private void SendPasswordViaSMS(string phoneNumber, string newPassword)
        {
            MessageResource.Create(
                body: $"Votre nouveau mot de passe est : {newPassword}. Veuillez vous connecter et le changer immédiatement.",
                from: new PhoneNumber(_twilioPhoneNumber),
                to: new PhoneNumber(phoneNumber)
            );
        }
    }
}
