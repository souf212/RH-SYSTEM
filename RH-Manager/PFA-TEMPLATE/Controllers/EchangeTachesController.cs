using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PFA_TEMPLATE.Data;
using PFA_TEMPLATE.Models;
using PFA_TEMPLATE.ViewModels;
using System.Security.Claims;

namespace PFA_TEMPLATE.Controllers
{
    public class EchangeTachesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EchangeTachesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Afficher la liste des tâches disponibles pour l'échange
        public async Task<IActionResult> Index()
        {
            // Récupérer l'ID de l'employé connecté
            var idUtilisateur = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var employe = await _context.Employes
                .FirstOrDefaultAsync(e => e.IdUtilisateur == idUtilisateur);

            if (employe == null)
                return NotFound("Employé non trouvé.");

            // Récupérer les tâches de l'employé connecté
            var mesTaches = await _context.Taches
                .Where(t => t.IdEmploye == employe.IdEmploye && (t.Statut == "Active" || t.Statut == "Pending"))
                .ToListAsync();

            // Récupérer les autres tâches disponibles pour échange (Active ou Pending, sauf celles de l'employé connecté)
            var autresTaches = await _context.Taches
       .Include(t => t.Employe)
           .ThenInclude(e => e.Utilisateur) // Ajoute ça 👈
       .Where(t => t.IdEmploye != employe.IdEmploye && (t.Statut == "Active" || t.Statut == "Pending"))
       .ToListAsync();

            foreach (var tache in autresTaches)
            {
                await _context.Entry(tache.Employe).Reference(e => e.Utilisateur).LoadAsync();
            }

            // Récupérer les demandes d'échange où l'employé est impliqué
            var demandesEchange = await _context.EchangeTaches
                .Include(e => e.TacheDemandeur)
                .Include(e => e.TacheDestinataire)
                .Include(e => e.EmployeDemandeur)
                .Include(e => e.EmployeDestinataire)
                .Where(e => e.IdEmployeDemandeur == employe.IdEmploye || e.IdEmployeDestinataire == employe.IdEmploye)
                .ToListAsync();

            ViewBag.MesTaches = mesTaches;
            ViewBag.AutresTaches = autresTaches;
            ViewBag.DemandesEchange = demandesEchange;
            ViewBag.IdEmploye = employe.IdEmploye;

            return View();
        }

        // Demander un échange de tâches
        [HttpPost]
        public async Task<IActionResult> DemanderEchange(EchangeTachesVM echangeVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Vérifier que les tâches et les employés existent
            var tacheDemandeur = await _context.Taches.FindAsync(echangeVM.IdTacheDemandeur);
            var tacheDestinataire = await _context.Taches.FindAsync(echangeVM.IdTacheDestinataire);

            if (tacheDemandeur == null || tacheDestinataire == null)
                return NotFound("Une des tâches n'existe pas.");

            // Créer la demande d'échange
            var echange = new EchangeTaches
            {
                IdTacheDemandeur = echangeVM.IdTacheDemandeur,
                IdTacheDestinataire = echangeVM.IdTacheDestinataire,
                IdEmployeDemandeur = tacheDemandeur.IdEmploye,
                IdEmployeDestinataire = tacheDestinataire.IdEmploye,
                Statut = "EnAttente",
                DateDemande = DateTime.Now
            };

            _context.EchangeTaches.Add(echange);
            await _context.SaveChangesAsync();

            // Créer une notification pour l'employé destinataire
            var notification = new Notification
            {
                Message = $"Nouvelle demande d'échange de tâche : '{tacheDemandeur.Titre}' contre '{tacheDestinataire.Titre}'",
                CreatedAt = DateTime.Now,
                IsRead = false,
                IdEmploye = tacheDestinataire.IdEmploye,
                IdTache = tacheDestinataire.IdTaches
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Accepter un échange
        [HttpPost]
        public async Task<IActionResult> AccepterEchange(int idEchange)
        {
            var echange = await _context.EchangeTaches
                .Include(e => e.TacheDemandeur)
                .Include(e => e.TacheDestinataire)
                .FirstOrDefaultAsync(e => e.IdEchange == idEchange);

            if (echange == null)
                return NotFound("Demande d'échange non trouvée.");

            // Vérifier que l'employé connecté est bien le destinataire
            var idUtilisateur = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var employe = await _context.Employes.FirstOrDefaultAsync(e => e.IdUtilisateur == idUtilisateur);

            if (employe == null || employe.IdEmploye != echange.IdEmployeDestinataire)
                return Forbid("Vous n'êtes pas autorisé à répondre à cette demande d'échange.");

            // Mettre à jour le statut de l'échange
            echange.Statut = "Accepte";
            echange.DateReponse = DateTime.Now;

            // Échanger les assignations des tâches
            var tacheDemandeur = await _context.Taches.FindAsync(echange.IdTacheDemandeur);
            var tacheDestinataire = await _context.Taches.FindAsync(echange.IdTacheDestinataire);

            if (tacheDemandeur != null && tacheDestinataire != null)
            {
                // Sauvegarder les IDs temporairement
                var idEmployeDemandeur = tacheDemandeur.IdEmploye;
                var idEmployeDestinataire = tacheDestinataire.IdEmploye;

                // Échanger les assignations
                tacheDemandeur.IdEmploye = idEmployeDestinataire;
                tacheDestinataire.IdEmploye = idEmployeDemandeur;

                // Mettre à jour la date de modification
                tacheDemandeur.LastModified = DateTime.Now;
                tacheDestinataire.LastModified = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            // Créer des notifications pour les deux employés
            var notificationDemandeur = new Notification
            {
                Message = $"Votre demande d'échange pour la tâche '{tacheDemandeur?.Titre}' a été acceptée.",
                CreatedAt = DateTime.Now,
                IsRead = false,
                IdEmploye = echange.IdEmployeDemandeur,
                IdTache = echange.IdTacheDemandeur
            };

            var notificationDestinataire = new Notification
            {
                Message = $"Vous avez accepté l'échange de tâche. Vous êtes maintenant responsable de '{tacheDemandeur?.Titre}'.",
                CreatedAt = DateTime.Now,
                IsRead = false,
                IdEmploye = echange.IdEmployeDestinataire,
                IdTache = echange.IdTacheDemandeur
            };

            _context.Notifications.AddRange(notificationDemandeur, notificationDestinataire);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Refuser un échange
        [HttpPost]
        public async Task<IActionResult> RefuserEchange(int idEchange, string? raisonRefus)
        {
            var echange = await _context.EchangeTaches
                .Include(e => e.TacheDemandeur)
                .FirstOrDefaultAsync(e => e.IdEchange == idEchange);

            if (echange == null)
                return NotFound("Demande d'échange non trouvée.");

            // Vérifier que l'employé connecté est bien le destinataire
            var idUtilisateur = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var employe = await _context.Employes.FirstOrDefaultAsync(e => e.IdUtilisateur == idUtilisateur);

            if (employe == null || employe.IdEmploye != echange.IdEmployeDestinataire)
                return Forbid("Vous n'êtes pas autorisé à répondre à cette demande d'échange.");

            // Mettre à jour le statut de l'échange
            echange.Statut = "Refuse";
            echange.DateReponse = DateTime.Now;
            echange.RaisonRefus = raisonRefus;

            await _context.SaveChangesAsync();

            // Créer une notification pour l'employé demandeur
            var notification = new Notification
            {
                Message = $"Votre demande d'échange pour la tâche '{echange.TacheDemandeur?.Titre}' a été refusée.",
                CreatedAt = DateTime.Now,
                IsRead = false,
                IdEmploye = echange.IdEmployeDemandeur,
                IdTache = echange.IdTacheDemandeur
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}