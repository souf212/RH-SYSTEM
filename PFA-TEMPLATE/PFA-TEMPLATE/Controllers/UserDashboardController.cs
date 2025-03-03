using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PFA_TEMPLATE.Controllers
{
    [Authorize(Roles = "User")] // Restrict access to standard users
    public class UserDashboardController : Controller
    {

        public IActionResult Index()
        {
            ViewData["Title"] = "User Dashboard";

            // Example data for the chart
            var taskProgress = new List<int> { 30, 50, 80, 45, 90 }; // Progress percentages
            ViewBag.TaskProgress = taskProgress;

            var taskNames = new List<string> { "Task 1", "Task 2", "Task 3", "Task 4", "Task 5" };
            ViewBag.TaskNames = taskNames;

            return View();
        }

        public IActionResult Profile()
        {
            ViewData["Title"] = "User Profile";
            // Add logic to fetch and display user profile
            return View();
        }

        public IActionResult Tasks()
        {
            ViewData["Title"] = "My Tasks";
            // Add logic to fetch and display user tasks
            return View();
        }
    }
}
