using Microsoft.EntityFrameworkCore;
using PFA_TEMPLATE.Data;
using PFA_TEMPLATE.Models;
using PFA_TEMPLATE.ViewModels;

namespace PFA_TEMPLATE.Services
{
    public class TacheService : ITacheService
    {
        private readonly ApplicationDbContext _context;

        public TacheService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<TachesVM> GetAllTaches()
        {
            return _context.Taches
                .Select(t => new TachesVM
                {
                    Id = t.IdTaches,
                    Title = t.Titre,
                    Description = t.Description,
                    AssignedTo = _context.Employes
                        .Where(e => e.IdEmploye == t.IdEmploye)
                        .Select(e => $"{e.Utilisateur.Nom} {e.Utilisateur.Prenom}")
                        .FirstOrDefault() ?? "Non assigné",
                    Status = t.Statut
                })
                .ToList();
        }
        public List<EmployeDropdownVM> GetEmployesForDropdown()
        {
            return _context.Employes
                .Select(e => new EmployeDropdownVM
                {
                    IdEmploye = e.IdEmploye,
                    Nom = $"{e.Utilisateur.Nom}   {e.Utilisateur.Prenom}"
                })
                .ToList();
        }
        public List<TachesVM> GetTachesByEmployee(string loggedInUser)
        {
            var employe = _context.Employes
                .Include(e => e.Utilisateur)
                .Include(e => e.Taches)
                .FirstOrDefault(e => e.Utilisateur.Login.ToLower() == loggedInUser);

            if (employe == null)
            {
                return new List<TachesVM>();
            }

            return employe.Taches
                .Select(t => new TachesVM
                {
                    Id = t.IdTaches,
                    Title = t.Titre,
                    Description = t.Description,
                    AssignedTo = $"{employe.Utilisateur.Nom}   {employe.Utilisateur.Prenom}",
                    Status = t.Statut
                })
                .ToList();
        }

        public void CreateTache(TachesVM tachesVM)
        {
            var tache = new Taches
            {
                Titre = tachesVM.Title,
                Description = tachesVM.Description,
                Statut = tachesVM.Status,
                IdEmploye = int.Parse(tachesVM.AssignedTo),
                CreatedAt = DateTime.Now,
                LastModified = DateTime.Now
            };

            _context.Taches.Add(tache);
            _context.SaveChanges();
        }

        public TachesVM GetTacheById(int id)
        {
            var tache = _context.Taches.Find(id);
            if (tache == null) return null;

            return new TachesVM
            {
                Id = tache.IdTaches,
                Title = tache.Titre,
                Description = tache.Description,
                AssignedTo = tache.IdEmploye.ToString(),
                Status = tache.Statut
            };
        }

        public void UpdateTache(TachesVM tachesVM)
        {
            var tache = _context.Taches.Find(tachesVM.Id);
            if (tache == null) return;

            tache.Titre = tachesVM.Title;
            tache.Description = tachesVM.Description;
            tache.Statut = tachesVM.Status;
            tache.IdEmploye = int.Parse(tachesVM.AssignedTo);

            _context.Taches.Update(tache);
            _context.SaveChanges();
        }

        public void UpdateTacheStatus(TachesVM model)
        {
            var tache = _context.Taches.Find(model.Id);
            if (tache == null) return;

            tache.Statut = model.Status;
            tache.LastModified = DateTime.Now;

            _context.SaveChanges();
        }

        public void DeleteTache(int id)
        {
            var tache = _context.Taches.Find(id);
            if (tache == null) return;

            _context.Taches.Remove(tache);
            _context.SaveChanges();
        }
    }
}