using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PFA_TEMPLATE.Data;
using PFA_TEMPLATE.Services;
using System.Security.Claims;

namespace PFA_TEMPLATE.Controllers
{
    public class NotificationController : Controller
    {
        private readonly NotificationService _notificationService;
        private readonly ApplicationDbContext _context; // Add DbContext

        public NotificationController(NotificationService notificationService, ApplicationDbContext context)
        {
            _notificationService = notificationService;
            _context = context; // Inject the DbContext
        }

        public async Task<IActionResult> Index()
        {
            // Get employee ID from the logged-in user
            int employeeId = await GetLoggedInEmployeeId();
            var notifications = await _notificationService.GetEmployeeNotifications(employeeId);
            return View(notifications);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkNotificationAsRead(id);
            return RedirectToAction("Index");
        }

        // Fix the method to be async and use the correct parameter and return type
        private async Task<int> GetLoggedInEmployeeId()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    var userId = userIdClaim.Value;
                    var employee = await _context.Employes
                        .FirstOrDefaultAsync(e => e.IdUtilisateur.ToString() == userId);
                    return employee?.IdEmploye ?? 0;
                }
            }
            return 0;
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            int employeeId = await GetLoggedInEmployeeId();
            var notifications = await _notificationService.GetEmployeeNotifications(employeeId);
            int unreadCount = notifications.Count(n => !n.IsRead);
            return Json(unreadCount);
        }

        [HttpGet]
        public async Task<IActionResult> GetLatestNotifications()
        {
            int employeeId = await GetLoggedInEmployeeId();
            var notifications = await _notificationService.GetEmployeeNotifications(employeeId);
            var latestNotifications = notifications
                .OrderByDescending(n => n.CreatedAt)
                .Take(5)
                .Select(n => new
                {
                    id = n.Id,
                    message = n.Message,
                    createdAt = n.CreatedAt,
                    isRead = n.IsRead,
                    tacheId = n.IdTache
                })
                .ToList();

            return Json(latestNotifications);
        }

        [HttpGet]
        public async Task<IActionResult> GoToNotification(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }

            // Mark as read
            await _notificationService.MarkNotificationAsRead(id);

            // Redirect to the task if available
            if (notification.IdTache.HasValue)
            {
                return RedirectToAction("Details", "Tache", new { id = notification.IdTache.Value });
            }

            // If no task is associated, redirect to notifications page
            return RedirectToAction("Index");
        }
    }
}