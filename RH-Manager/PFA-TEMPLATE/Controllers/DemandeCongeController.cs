using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PFA_TEMPLATE.Data;
using PFA_TEMPLATE.Mappers;
using PFA_TEMPLATE.Models;
using PFA_TEMPLATE.ViewModels;

namespace PFA_TEMPLATE.Controllers
{
    public class DemandeCongeController : Controller
    {
        public readonly ApplicationDbContext _Context;
        public DemandeCongeController(ApplicationDbContext context)
        {
            _Context = context;
        }
        public async Task<IActionResult> Index()
        {
            // Get the currently logged-in employee's ID
            var currentEmployeeId = await GetCurrentEmployeeIdAsync();

            // If no valid employee found, return an appropriate response
            if (currentEmployeeId == -1)
            {
                // Option 1: Return an empty list with a message
                TempData["EmployeeMessage"] = "Utilisateur non identifié.";
                return View(new List<Conges>());

                // Option 2: Redirect to login or error page
                // return RedirectToAction("Login", "Account");
            }

            // Filter leave requests to show only for the current employee
            var demandes = await _Context.Conges
                .Where(c => c.IdEmploye == currentEmployeeId)
                .OrderByDescending(c => c.DateDebut) // Optional: Sort by most recent first
                .ToListAsync();

            return View(demandes);
        }

        // Helper method to get the current employee's ID
        private async Task<int> GetCurrentEmployeeIdAsync()
        {
            var usernameClaim = User.Identity?.Name;
            if (string.IsNullOrEmpty(usernameClaim))
            {
                return -1;
            }

            try
            {
                var utilisateur = await _Context.Utilisateurs
                    .FirstOrDefaultAsync(u => u.Login == usernameClaim);

                if (utilisateur == null)
                {
                    return -1;
                }

                // Find the associated employee
                var employee = await _Context.Employes
                    .FirstOrDefaultAsync(e => e.IdUtilisateur == utilisateur.Id);

                return employee?.IdEmploye ?? -1;
            }
            catch (Exception ex)
            {
                // Log the exception
                // _logger.LogError(ex, "Error retrieving employee ID");
                return -1;
            }
        }

        public IActionResult CreateConge()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateConge(DemandeCongeViewModels vm)
        {
            var currentUserLogin = HttpContext.User.Identity.Name;

            var utilisateur = await _Context.Utilisateurs
                                             .FirstOrDefaultAsync(u => u.Login == currentUserLogin);

            if (utilisateur == null)
            {
                ModelState.AddModelError("", "Utilisateur introuvable.");
                return View(vm);
            }

            var employe = await _Context.Employes
                                        .FirstOrDefaultAsync(e => e.IdUtilisateur == utilisateur.Id);

            if (employe == null)
            {
                ModelState.AddModelError("", "Employé introuvable.");
                return View(vm);
            }

            if (ModelState.IsValid)
            {
                var conge = DemandeCongeMapper.GetCongesFromDemandeCongeViewModel(vm);
                conge.IdEmploye = employe.IdEmploye;
                conge.Status = "En attente";

                _Context.Conges.Add(conge);
                await _Context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int IdConges)
        {
            var conge = await _Context.Conges.FindAsync(IdConges);
            if (conge == null)
            {
                return NotFound();
            }

            _Context.Conges.Remove(conge);
            await _Context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        // ADMIN : Afficher toutes les demandes
        public async Task<IActionResult> AdminIndex()
        {
            var demandes = await _Context.Conges
    .Include(c => c.Employe)
    .ThenInclude(e => e.Utilisateur) // Assure que l'utilisateur est chargé
    .ToListAsync();

            foreach (var conge in demandes)
            {
                _Context.Entry(conge).Reference(c => c.Employe).Load();
                _Context.Entry(conge.Employe).Reference(e => e.Utilisateur).Load();
                if (conge == null)
                {
                    Console.WriteLine("Congé est null");
                    continue;
                }

                if (conge.Employe == null)
                {
                    Console.WriteLine($"IdConges: {conge.IdConges}, Employe: Employé non chargé");
                }
                else
                {
                    Console.WriteLine($"IdConges: {conge.IdConges}, Employe: {conge.Employe.NomComplet}");
                }
            }

            return View(demandes); // Teste la sortie JSON pour voir si les noms complets sont récupérés
        }

        // ADMIN : Accepter ou refuser une demande
        [HttpPost]
        public async Task<IActionResult> Decision(int id, string status, string? comment)
        {
            var conge = await _Context.Conges.Include(c => c.Employe).FirstOrDefaultAsync(c => c.IdConges == id);
            if (conge == null)
            {
                return NotFound();
            }

            // Mise à jour du statut et commentaire
            conge.Status = status;
            conge.AdminComment = comment;

            _Context.Conges.Update(conge);
            await _Context.SaveChangesAsync();

            // Message pour l'employé
            TempData["EmployeeMessage"] = $"Votre demande de congé a été {status.ToLower()}.";
            TempData["Message"] = $"Demande de congé {status.ToLower()} avec succès.";

            return RedirectToAction("AdminIndex");
        }





    }
}
