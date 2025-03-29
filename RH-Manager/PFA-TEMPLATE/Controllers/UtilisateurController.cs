using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PFA_TEMPLATE.Data;
using PFA_TEMPLATE.Services;
using PFA_TEMPLATE.ViewModels;
using static PFA_TEMPLATE.Services.UserService;

namespace PFA_TEMPLATE.Controllers
{
    public class UtilisateurController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UtilisateurController> _logger;
        private readonly ApplicationDbContext _context;
        public UtilisateurController(
            IUserService userService, ApplicationDbContext context,
            ILogger<UtilisateurController> logger)
        {
            _userService = userService;
            _logger = logger;
            _context = context;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id); // Assume this method exists in IUserService
            if (user == null)
                return NotFound();
            return Ok(user);
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserVM userVM)
        {
            try
            {
                var createdUser = await _userService.CreateUserAsync(userVM);
                return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
            }
            catch (UserCreationException ex)
            {
                return StatusCode(500, "An error occurred while creating the user");
            }
            catch (InvalidRoleException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: Utilisateur
        public async Task<IActionResult> Index()
        {
            // Get the current user's name from claims
            var currentUserName = User.Identity.Name;

            // If the user is an Admin, show all users
            if (User.IsInRole("Admin"))
            {
                var users = await _userService.GetAllUsersAsync();
                return View(users);
            }

            // If the user is not an Admin, show only their own information
            var allUsers = await _userService.GetAllUsersAsync();
            var currentUser = allUsers.FirstOrDefault(u => u.Login == currentUserName);

            if (currentUser != null)
            {
                return View(new List<UserVM> { currentUser });
            }

            return View(new List<UserVM>());
        }

        public async Task<IActionResult> Details(int? id)
        {
            // Get the current user's name from claims
            var currentUserName = User.Identity.Name;

            // If the user is not an Admin or no id is provided, show the current user's details
            if (!User.IsInRole("Admin") || id == null)
            {
                // Get all users and find the current one by username
                var allUsers = await _userService.GetAllUsersAsync();
                var currentUser = allUsers.FirstOrDefault(u => u.Login == currentUserName);

                if (currentUser == null)
                {
                    return NotFound();
                }

                return View(currentUser);
            }

            // If the user is an admin and provided an id, show that user's details
            var user = await _userService.GetUserByIdAsync(id.Value);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        [HttpGet]
        public async Task<IActionResult> CheckEmail(string email)
        {
            var exists = await _context.Utilisateurs
                .AnyAsync(u => u.Email.ToLower() == email.ToLower());
            return Json(!exists);
        }

        [HttpGet]
        public async Task<IActionResult> CheckLogin(string login)
        {
            var exists = await _context.Utilisateurs
                .AnyAsync(u => u.Login.ToLower() == login.ToLower());
            return Json(!exists);
        }

        [HttpGet]
        public async Task<IActionResult> CheckCIN(string cin)
        {
            var exists = await _context.Utilisateurs
                .AnyAsync(u => u.CIN.ToLower() == cin.ToLower());
            return Json(!exists);
        }
        // GET: Utilisateur/Create
        public IActionResult Create()
        {
            return View(new UserVM());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserVM userVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _userService.CreateUserAsync(userVM);
                    TempData["Success"] = "Utilisateur créé avec succès";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (InvalidOperationException ex)
            {
                // Handle duplicate cases specifically
                if (ex.Message.Contains("email"))
                    ModelState.AddModelError("Email", ex.Message);
                else if (ex.Message.Contains("login"))
                    ModelState.AddModelError("Login", ex.Message);
                else if (ex.Message.Contains("CIN"))
                    ModelState.AddModelError("CIN", ex.Message);
                else
                    ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Une erreur est survenue lors de la création");
                _logger.LogError(ex, "Error creating user");
            }

            return View(userVM);
        }
        // GET: Utilisateur/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserByIdAsync(id.Value);
            if (user == null)
            {
                return NotFound();
            }

            // Clear password for security reasons
            user.Password = string.Empty;

            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserVM userVM, bool isAdminEdit = true)
        {
            if (id != userVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bool success = await _userService.UpdateUserAsync(userVM);
                    if (success)
                    {
                        TempData["SuccessMessage"] = isAdminEdit
                            ? "Utilisateur modifié avec succès!"
                            : "Vos informations modifiées avec succès!";

                        return isAdminEdit
                            ? RedirectToAction(nameof(Index))
                            : RedirectToAction(nameof(Details));
                    }
                    else
                    {
                        ModelState.AddModelError("", "La mise à jour a échoué.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Une erreur est survenue: {ex.Message}");
                }
            }
            return View(userVM);
        }

        // GET: Utilisateur/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserByIdAsync(id.Value);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Utilisateur/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                bool success = await _userService.DeleteUserAsync(id);
                if (success)
                {
                    TempData["SuccessMessage"] = "Utilisateur supprimé avec succès!";
                }
                else
                {
                    TempData["ErrorMessage"] = "La suppression a échoué.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Une erreur est survenue: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
