using Microsoft.AspNetCore.Mvc;
using PFA_TEMPLATE.Services;
using PFA_TEMPLATE.ViewModels;

namespace PFA_TEMPLATE.Controllers
{
    public class UtilisateurController : Controller
    {
        private readonly IUserService _userService;

        public UtilisateurController(IUserService userService)
        {
            _userService = userService;
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

        // GET: Utilisateur/Create
        public IActionResult Create()
        {
            return View(new UserVM());
        }

        // POST: Utilisateur/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserVM userVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _userService.CreateUserAsync(userVM);
                    TempData["SuccessMessage"] = "Utilisateur créé avec succès!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Une erreur est survenue: {ex.Message}");
                }
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
        public async Task<IActionResult> Edit1(int? id)
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

        // POST: Utilisateur/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserVM userVM)
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
                        TempData["SuccessMessage"] = "Utilisateur modifié avec succès!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "La mise à jour a échoué pour une raison inconnue.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Une erreur est survenue: {ex.Message}");
                }
            }
            return View(userVM);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit1(int id, UserVM userVM)
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
                        TempData["SuccessMessage"] = "Vos informations modifié avec succès!";
                        return RedirectToAction(nameof(Details));
                    }
                    else
                    {
                        ModelState.AddModelError("", "La mise à jour a échoué pour une raison inconnue.");
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