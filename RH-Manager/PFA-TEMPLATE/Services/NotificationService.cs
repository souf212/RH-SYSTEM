using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PFA_TEMPLATE.Data;
using PFA_TEMPLATE.Hubs;
using PFA_TEMPLATE.Models;

namespace PFA_TEMPLATE.Services
{
    public class NotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(ApplicationDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task CreateTaskNotification(Taches tache)
        {
            try
            {
                LogInformation($"Creating notification for task {tache.IdTaches} assigned to employee {tache.IdEmploye}");

                var notification = new Notification
                {
                    Message = $"Une nouvelle tâche '{tache.Titre}' vous a été assignée",
                    IdEmploye = tache.IdEmploye,
                    IdTache = tache.IdTaches,
                    CreatedAt = DateTime.Now
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
                LogInformation($"Notification created with ID {notification.Id}");

                // Prepare the notification data
                var notificationData = new
                {
                    id = notification.Id,
                    message = notification.Message,
                    createdAt = notification.CreatedAt,
                    tacheId = tache.IdTaches,
                    isRead = false
                };

                // Send real-time notification to the employee
                await _hubContext.Clients.Group(tache.IdEmploye.ToString())
                    .SendAsync("ReceiveNotification", notificationData);

                LogInformation($"SignalR notification sent to group {tache.IdEmploye}");
            }
            catch (Exception ex)
            {
                LogError($"Error creating or sending notification: {ex.Message}");
                throw; // Re-throw the exception if you want it to be handled upstream
            }
        }

        private void LogInformation(string message)
        {
            // Implement your logging mechanism here
            Console.WriteLine($"[INFO] {message}");
        }

        private void LogError(string message)
        {
            // Implement your logging mechanism here
            Console.WriteLine($"[ERROR] {message}");
        }



        public async Task<List<Notification>> GetEmployeeNotifications(int employeeId)
        {
            return await _context.Notifications
                .Where(n => n.IdEmploye == employeeId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkNotificationAsRead(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}