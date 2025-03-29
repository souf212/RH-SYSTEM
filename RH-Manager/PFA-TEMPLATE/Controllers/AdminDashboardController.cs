using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PFA_TEMPLATE.Data;
using PFA_TEMPLATE.ViewModels;

namespace PFA_TEMPLATE.Controllers
{
    [Authorize(Roles = "Admin")] // Restrict access to admin users
    public class AdminDashboardController : Controller
    {
        public readonly ApplicationDbContext _Context;
        public AdminDashboardController(ApplicationDbContext context)
        {
            _Context = context;
        }



        public async Task<IActionResult> Index()
        {
            // Fetch leave requests
            var demandes = await _Context.Conges
                .Include(c => c.Employe)
                    .ThenInclude(e => e.Utilisateur) // Ensure user is loaded
                .ToListAsync();

            // Logging
            Console.WriteLine($"Total Leave Requests: {demandes.Count}");

            // Get the total number of employees
            int totalEmployees = await _Context.Employes.CountAsync();

            // Get the total number of active tasks
            int activeTasks = await _Context.Taches
                .Where(t => t.Statut == "Active") // Filter active tasks
                .CountAsync();


            var tasks = await _Context.Taches
        .Include(t => t.Employe) // Include Employes
            .ThenInclude(e => e.Utilisateur) // Include Utilisateurs
        .Where(t => t.Statut == "Completed") // Filter completed tasks
        .Select(t => new TachesVM
        {
            Id = t.IdTaches,
            Title = t.Titre,
            Description = t.Description,
            AssignedTo = t.Employe.Utilisateur.Nom + " " + t.Employe.Utilisateur.Prenom, // Combine Nom and Prenom
            Status = t.Statut
        })
        .ToListAsync();
            // Create the ViewModel
            var viewModel = new IndexViewModel
            {
                LeaveRequests = demandes,
                TotalEmployees = totalEmployees,
                ActiveTasks = activeTasks,
                TotalLeaveRequests = demandes.Count,
                CompletedTasks = tasks  // Add leave requests count
            };
            // Pass the ViewModel to the view
            return View(viewModel);
        }

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


        public IActionResult ManageUsers()
        {
            ViewData["Title"] = "Manage Users";
            // Add logic to manage users
            return View();
        }

        public IActionResult Reports()
        {
            ViewData["Title"] = "Admin Reports";
            // Add logic to display reports
            return View();
        }
    }
}
