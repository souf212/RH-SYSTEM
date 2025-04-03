using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PFA_TEMPLATE.Data;
using PFA_TEMPLATE.Models;
using PFA_TEMPLATE.Services;

namespace PFA_TEMPLATE.Controllers
{
    public class FichesPaieController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly FicheDePaieService _ficheService;

        public FichesPaieController(ApplicationDbContext context)
        {
            _context = context;
            _ficheService = new FicheDePaieService(_context);
        }

        // ✅ Admin : Générer toutes les fiches pour ce mois
        public async Task<IActionResult> GenererTout()
        {
            var utilisateurs = await _context.Utilisateurs
                .Where(u => u.Role == "Employes")
                .ToListAsync();

            int count = 0;

            foreach (var utilisateur in utilisateurs)
            {
                var fiche = await _ficheService.GenererFicheMensuelleAsync(utilisateur.Id);
                if (fiche != null) count++;
            }

            TempData["success"] = $"{count} fiche(s) générée(s) avec succès.";
            return RedirectToAction("Index");
        }

        // ✅ Liste de toutes les fiches
        public IActionResult Index()
        {
            var fiches = _context.FichesDePaie
                .Include(f => f.Contrat)
                .ThenInclude(c => c.Utilisateur)
                .OrderByDescending(f => f.Date)
                .ToList();

            return View(fiches);
        }
    }
}
