using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PFA_TEMPLATE.Data;
using PFA_TEMPLATE.ViewModels;

namespace PFA_TEMPLATE.Controllers
{
    public class PointageMvcController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PointageMvcController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Affichage des pointages pour une date sélectionnée
        public IActionResult Index(DateTime? date)
        {
            DateTime selectedDate = date ?? DateTime.Today;

            var employesAvecPointage = _context.Employes
                .Include(e => e.Utilisateur) // 🔥 Fetch the user data to get the name
                .Select(e => new PointageViewModel
                {
                    IdEmploye = e.IdEmploye,
                    NomEmploye = e.Utilisateur != null ? e.Utilisateur.Nom : "Nom non disponible", // ✅ Get name from Utilisateurs
                    HeureEntree = _context.Pointages
                        .Where(p => p.IdEmploye == e.IdEmploye && p.HeureEntree.Date == selectedDate.Date)
                        .OrderByDescending(p => p.HeureEntree)
                        .Select(p => (DateTime?)p.HeureEntree)
                        .FirstOrDefault(),

                    HeureSortie = _context.Pointages
                        .Where(p => p.IdEmploye == e.IdEmploye && p.HeureEntree.Date == selectedDate.Date)
                        .OrderByDescending(p => p.HeureEntree)
                        .Select(p => (DateTime?)p.HeureSortie)
                        .FirstOrDefault(),

                    // Déterminer le statut
                    Statut = _context.Pointages
                        .Any(p => p.IdEmploye == e.IdEmploye && p.HeureEntree.Date == selectedDate.Date)
                        ? (_context.Pointages.Any(p => p.IdEmploye == e.IdEmploye && p.HeureSortie == null && p.HeureEntree.Date == selectedDate.Date)
                            ? "Présent"
                            : "Absent")
                        : "Absent"
                })
                .ToList();

            ViewBag.SelectedDate = selectedDate.ToString("yyyy-MM-dd");
            return View(employesAvecPointage);
        }

        // ✅ Afficher l'historique d'un employé
        public IActionResult Historique(int id)
        {
            var historique = _context.Pointages
                .Where(p => p.IdEmploye == id)
                .OrderByDescending(p => p.HeureEntree)
                .Select(p => new PointageViewModel
                {
                    IdEmploye = p.IdEmploye,
                    HeureEntree = p.HeureEntree,
                    HeureSortie = p.HeureSortie,
                    Statut = p.HeureSortie == null ? "Présent" : "Absent"
                })
                .ToList();

            ViewBag.Employe = _context.Employes
                .Include(e => e.Utilisateur) // ✅ Ensure the user is loaded
                .FirstOrDefault(e => e.IdEmploye == id);

            return View(historique);
        }
    }
}
