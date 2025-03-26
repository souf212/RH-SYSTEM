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
namespace PFA_TEMPLATE.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _vonageApiKey;
        private readonly string _vonageApiSecret;
        private readonly VonageClient _vonageClient;

        public AccountController(
            ApplicationDbContext context,
            IConfiguration configuration,
    VonageClient vonageClient)

        {
            _vonageClient = vonageClient;
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
                await SendPasswordViaSMS(user.Telephone, newPassword);
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

        private string NormalizePhoneNumber(string phoneNumber)
        {
            // Strict Moroccan number validation
            var digitsOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());

            // Ensure it's a valid Moroccan number
            if (digitsOnly.Length == 9 && digitsOnly.StartsWith("6"))
            {
                return $"212{digitsOnly}";
            }

            if (digitsOnly.Length == 10 && digitsOnly.StartsWith("06"))
            {
                return $"212{digitsOnly.Substring(1)}";
            }

            throw new ArgumentException("Invalid Moroccan phone number format");
        }

        private async Task SendPasswordViaSMS(string phoneNumber, string newPassword)
        {
            try
            {
                string formattedPhoneNumber = NormalizePhoneNumber(phoneNumber);

                // Use the SMS API client instead of direct SendSmsAsync method
                var smsClient = _vonageClient.SmsClient;

                var request = new SendSmsRequest
                {
                    From = "YourCompanyName",
                    To = formattedPhoneNumber,
                    Text = $"Votre nouveau mot de passe est : {newPassword}."
                };
            }
            // Use the SMS client to send the message
            /*  var response = await smsClient.SendSmsAsync(request);

              // Log the SMS send result
              if (response.Messages[0].Status == "0")
              {
                  Console.WriteLine($"SMS sent successfully. Message ID: {response.Messages[0].MessageId}");
              }
              else
              {
                  Console.WriteLine($"SMS send failed. Error: {response.Messages[0].ErrorText}");
                  throw new Exception($"SMS send failed: {response.Messages[0].ErrorText}");
              }
          }*/
            catch (Exception ex)
            {
                Console.WriteLine($"SMS Send Error: {ex.Message}");
                throw;
            }
          }
            }
}
    

