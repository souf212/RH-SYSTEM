using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR_Management_System.Controllers
{
    [Authorize(Roles = "Admin")] // Restrict access to admin users
    public class AdminDashboardController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Admin Dashboard";

            // Example data for the chart
            var userSignups = new List<int> { 65, 59, 80, 81, 56, 55, 40 };
            ViewBag.UserSignups = userSignups;

            var months = new List<string> { "January", "February", "March", "April", "May", "June", "July" };
            ViewBag.Months = months;

            return View();
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
