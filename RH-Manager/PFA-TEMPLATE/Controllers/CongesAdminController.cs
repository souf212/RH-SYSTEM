using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PFA_TEMPLATE.Data;
using PFA_TEMPLATE.Models;

namespace PFA_TEMPLATE.Controllers
{
    public class CongesAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CongesAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🔎 Liste de toutes les demandes de congé
        public async Task<IActionResult> Index()
        {
            var demandes = await _context.Conges
                .Include(c => c.Employe)
                .ThenInclude(e => e.Utilisateur)
                .OrderByDescending(c => c.DateDebut)
                .ToListAsync();

            return View(demandes);
        }

        // ✅ Traitement de validation ou refus
        [HttpPost]
        public async Task<IActionResult> Valider(int id, string action, string commentaire)
        {
            var conge = await _context.Conges
                .Include(c => c.Employe)
                .FirstOrDefaultAsync(c => c.IdConges == id);

            if (conge == null)
                return NotFound();

            conge.AdminComment = commentaire;

            if (action == "Accepter")
            {
                conge.Status = "Accepté";

                // Mise à jour du solde si accepté
                var balance = await _context.CongeBalances
                    .FirstOrDefaultAsync(b => b.IdEmploye == conge.IdEmploye && b.Annee == DateTime.Now.Year);

                if (balance != null)
                {
                    var joursDemandes = (conge.DateFin - conge.DateDebut).TotalDays + 1;
                    balance.JoursCongesPayesUtilises += (decimal)joursDemandes;
                    _context.CongeBalances.Update(balance);
                }
            }
            else if (action == "Refuser")
            {
                conge.Status = "Refusé";
            }

            _context.Conges.Update(conge);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
