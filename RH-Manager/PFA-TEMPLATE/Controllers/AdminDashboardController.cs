using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PFA_TEMPLATE.Data;

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

        public async Task<IActionResult> Dashboard()
        {
            // Get total number of employees
            int totalEmployees = await _Context.Employes.CountAsync();

            int lastPeriodEmployees = GetLastPeriodEmployeeCount();
            double growthPercentage = CalculateGrowthPercentage(totalEmployees, lastPeriodEmployees);

            var dashboardViewModel = new DashboardViewModel
            {
                TotalEmployees = totalEmployees,
                EmployeeGrowthPercentage = growthPercentage
            };

            return View(dashboardViewModel);
        }

        // Helper method to calculate growth percentage
        private double CalculateGrowthPercentage(int currentCount, int previousCount)
        {
            if (previousCount == 0) return 0;
            return Math.Round(((double)(currentCount - previousCount) / previousCount) * 100, 1);
        }

        // Method to get previous period's employee count 
        // You'll need to implement this based on your specific tracking method
        private int GetLastPeriodEmployeeCount()
        {
            // Placeholder - replace with your actual logic
            // This could be:
            // - A stored value in a configuration table
            // - A manual entry
            // - A previous snapshot of employees
            return 220; // Example hardcoded value
        }



        public async Task<IActionResult> Index()
        {
            var demandes = await _Context.Conges
                .Include(c => c.Employe)
                    .ThenInclude(e => e.Utilisateur) // Ensure user is loaded
                .ToListAsync();

            // Logging
            Console.WriteLine($"Total Leave Requests: {demandes.Count}");

            return View(demandes);
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
