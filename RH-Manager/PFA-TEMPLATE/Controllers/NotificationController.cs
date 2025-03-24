using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PFA_TEMPLATE.Data;
using PFA_TEMPLATE.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace PFA_TEMPLATE.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Approach 1: If you're using ASP.NET Identity and the Utilisateur.Id is stored in the claim
            if (User.Identity.IsAuthenticated)
            {
                // Try to get the user ID
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out int userId))
                {
                    // Find the corresponding employee ID
                    var employeeId = await _context.Employes
                        .Where(e => e.IdUtilisateur == userId)
                        .Select(e => e.IdEmploye)
                        .FirstOrDefaultAsync();

                    if (employeeId != 0) // If we found the employee
                    {
                        // Get notifications for this employee
                        var notifications = await _context.Notifications
                            .Where(n => n.IdEmploye == employeeId)
                            .OrderByDescending(n => n.CreatedAt)
                            .ToListAsync();

                        return View(notifications);
                    }
                }
            }

            // Approach 2: Direct query for testing - use this to verify if there are notifications
            // Remove or comment this out in production
            var allNotifications = await _context.Notifications
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return View(allNotifications);

            // If no notifications found or user not authenticated properly
            // return View(new List<Notification>());
        }


        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            if (User.Identity.IsAuthenticated)
            {
                Console.WriteLine("✅ User is authenticated.");
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Console.WriteLine($"🔍 User ID from claims: {userIdString}");

                if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out int userId))
                {
                    Console.WriteLine($"🔍 Parsed User ID: {userId}");
                    var employeeId = await _context.Employes
                        .Where(e => e.IdUtilisateur == userId)
                        .Select(e => e.IdEmploye)
                        .FirstOrDefaultAsync();

                    Console.WriteLine($"🔍 Employee ID: {employeeId}");

                    if (employeeId != 0)
                    {
                        var count = await _context.Notifications
                            .Where(n => n.IdEmploye == employeeId && !n.IsRead)
                            .CountAsync();

                        Console.WriteLine($"🔍 Unread notifications count: {count}");
                        return Json(new { count });
                    }
                }
            }

            Console.WriteLine("❌ User is not authenticated or no unread notifications.");
            return Json(new { count = 0 });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllAsRead()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out int userId))
                {
                    var employeeId = await _context.Employes
                        .Where(e => e.IdUtilisateur == userId)
                        .Select(e => e.IdEmploye)
                        .FirstOrDefaultAsync();

                    if (employeeId != 0)
                    {
                        var notifications = await _context.Notifications
                            .Where(n => n.IdEmploye == employeeId && !n.IsRead)
                            .ToListAsync();

                        foreach (var notification in notifications)
                        {
                            notification.IsRead = true;
                        }

                        await _context.SaveChangesAsync();
                        return Json(new { success = true });
                    }
                }
            }

            return Json(new { success = false });
        }
    }
    }