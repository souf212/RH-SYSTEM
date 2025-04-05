using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PFA_TEMPLATE.Controllers
{
    [Authorize(Roles = "Employes")] // Restrict access to standard users
    public class UserDashboardController : Controller
    {

        public IActionResult Index()
        {
            ViewData["Title"] = "User Dashboard";
             
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
