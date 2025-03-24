using Microsoft.EntityFrameworkCore;
using PFA_TEMPLATE.Data;
using PFA_TEMPLATE.Models;
using PFA_TEMPLATE.Services;
using PFA_TEMPLATE.ViewModels;

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

    public async Task CreateTache(TachesVM tachesVM)
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
        await _context.SaveChangesAsync();

        // Create notification for the employee - make sure we're using the correct employee ID
        var notification = new Notification
        {
            Message = $"You have been assigned a new task: {tachesVM.Title}",
            CreatedAt = DateTime.Now,
            IsRead = false,
            IdEmploye = int.Parse(tachesVM.AssignedTo), // This should be the Employes.IdEmploye
            IdTache = tache.IdTaches
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
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

    // Méthode corrigée avec async/await et Task comme type de retour
    public async Task UpdateTache(TachesVM tachesVM)
    {
        var tache = _context.Taches.Find(tachesVM.Id);
        if (tache == null) return;

        tache.Titre = tachesVM.Title;
        tache.Description = tachesVM.Description;
        tache.Statut = tachesVM.Status;
        tache.IdEmploye = int.Parse(tachesVM.AssignedTo);
        tache.LastModified = DateTime.Now;

        _context.Taches.Update(tache);
        await _context.SaveChangesAsync();
    }

    // Méthode corrigée avec async/await et Task comme type de retour
    public async Task UpdateTacheStatus(TachesVM model)
    {
        var tache = _context.Taches.Find(model.Id);
        if (tache == null) return;

        tache.Statut = model.Status;
        tache.LastModified = DateTime.Now;

        await _context.SaveChangesAsync();
    }

    // Méthode corrigée avec async/await et Task comme type de retour
    public async Task DeleteTache(int id)
    {
        var tache = _context.Taches.Find(id);
        if (tache == null) return;

        // First, find and delete all notifications related to this task
        var relatedNotifications = _context.Notifications.Where(n => n.IdTache == id);
        _context.Notifications.RemoveRange(relatedNotifications);

        // Then delete the task itself
        _context.Taches.Remove(tache);

        await _context.SaveChangesAsync();
    }
}