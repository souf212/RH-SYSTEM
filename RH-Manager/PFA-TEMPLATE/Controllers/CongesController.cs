using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PFA_TEMPLATE.Data;
using PFA_TEMPLATE.Models;
using PFA_TEMPLATE.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

namespace PFA_TEMPLATE.Controllers
{
    [Authorize(Roles = "Employes")] // Only employees can access this controller
    public class CongesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CongesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Get user ID from claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var employe = await _context.Employes
                .Include(e => e.Utilisateur)
                .FirstOrDefaultAsync(e => e.IdUtilisateur == int.Parse(userId));

            if (employe == null)
                return NotFound("Employé non trouvé.");

            var demandes = await _context.Conges
                .Where(c => c.IdEmploye == employe.IdEmploye)
                .OrderByDescending(c => c.DateDebut)
                .ToListAsync();

            var model = new DemandeCongeViewModel
            {
                IdEmploye = employe.IdEmploye,
                ListeDemandes = demandes
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DemanderConge(DemandeCongeViewModel model)
        {
            // Get user ID from claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var employe = await _context.Employes
                .FirstOrDefaultAsync(e => e.IdUtilisateur == int.Parse(userId));

            if (employe == null)
                return NotFound("Employé non trouvé.");

            model.IdEmploye = employe.IdEmploye;

            if (!ModelState.IsValid)
            {
                // Reload the list of requests if validation fails
                model.ListeDemandes = await _context.Conges
                    .Where(c => c.IdEmploye == employe.IdEmploye)
                    .OrderByDescending(c => c.DateDebut)
                    .ToListAsync();
                return View("Index", model);
            }

            var balance = await _context.CongeBalances
                .FirstOrDefaultAsync(b => b.IdEmploye == employe.IdEmploye && b.Annee == DateTime.Now.Year);

            if (balance == null)
            {
                ModelState.AddModelError("", "Solde de congé non disponible.");
                model.ListeDemandes = await _context.Conges
                    .Where(c => c.IdEmploye == employe.IdEmploye)
                    .OrderByDescending(c => c.DateDebut)
                    .ToListAsync();
                return View("Index", model);
            }

            var joursDemandes = (model.DateFin - model.DateDebut).TotalDays + 1;

            if (joursDemandes > (double)balance.JoursCongesPayesRestants)
            {
                ModelState.AddModelError("", "Solde de congés insuffisant.");
                model.ListeDemandes = await _context.Conges
                    .Where(c => c.IdEmploye == employe.IdEmploye)
                    .OrderByDescending(c => c.DateDebut)
                    .ToListAsync();
                return View("Index", model);
            }

            var chevauchement = await _context.Conges
                .AnyAsync(c =>
                    c.IdEmploye == employe.IdEmploye &&
                    c.DateDebut <= model.DateFin &&
                    c.DateFin >= model.DateDebut &&
                    c.Status != "Refusé");

            if (chevauchement)
            {
                model.SuggestionDateDebut = model.DateDebut.AddDays(7);
                model.SuggestionDateFin = model.DateFin.AddDays(7);
                model.PeriodeSuggeree = true;
                model.ListeDemandes = await _context.Conges
                    .Where(c => c.IdEmploye == employe.IdEmploye)
                    .OrderByDescending(c => c.DateDebut)
                    .ToListAsync();

                ModelState.AddModelError("", "Vous avez déjà une demande de congé pendant cette période.");
                return View("Index", model);
            }

            var conge = new Conges
            {
                IdEmploye = employe.IdEmploye,
                Motif = model.Motif,
                DateDebut = model.DateDebut,
                DateFin = model.DateFin,
                Status = "En attente"
            };

            _context.Conges.Add(conge);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Votre demande a été soumise avec succès.";

            // Reload the view with updated data
            var nouveauModel = new DemandeCongeViewModel
            {
                IdEmploye = employe.IdEmploye,
                ListeDemandes = await _context.Conges
                    .Where(c => c.IdEmploye == employe.IdEmploye)
                    .OrderByDescending(c => c.DateDebut)
                    .ToListAsync()
            };

            return View("Index", nouveauModel);
        }
    }
}