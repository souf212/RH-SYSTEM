using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PFA_TEMPLATE.Data;
using System.Security.Claims; // ✅ à ajouter si tu veux utiliser les claims

namespace PFA_TEMPLATE.Controllers
{
    public class MonContratController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MonContratController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // ✅ OPTION 1 : via Login (comme tu faisais)
            var utilisateur = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.Login == User.Identity.Name);

            // ✅ OPTION 2 (meilleure) : via email (si User.Identity.Name contient l'email)
            // var email = User.Identity.Name;
            // var utilisateur = await _context.Utilisateurs
            //     .FirstOrDefaultAsync(u => u.Email == email);

            if (utilisateur == null)
                return NotFound("Utilisateur non connecté.");

            var contrat = await _context.Contrats
                .FirstOrDefaultAsync(c => c.IdUtilisateur == utilisateur.Id);

            if (contrat == null)
            {
                ViewBag.HasContrat = false;
                ViewBag.NomUtilisateur = utilisateur.Prenom + " " + utilisateur.Nom;
            }
            else
            {
                ViewBag.HasContrat = true;
                ViewBag.ContratId = contrat.IdContrat;
                ViewBag.NomUtilisateur = utilisateur.Prenom + " " + utilisateur.Nom;
            }

            return View();
        }
    }
}
