using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PFA_TEMPLATE.Data;
using PFA_TEMPLATE.Models;
using PFA_TEMPLATE.Services;
using PFA_TEMPLATE.ViewModels;
using PusherServer;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PFA_TEMPLATE.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PusherService _pusherService;
        private readonly IConfiguration _configuration;

        public MessagesController(ApplicationDbContext context, PusherService pusherService, IConfiguration configuration)
        {
            _context = context;
            _pusherService = pusherService;
            _configuration = configuration;
        }
        [HttpGet("Messages/Index")]
        public async Task<IActionResult> Index()
        {
             int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Get the employee information for the current user
            var currentEmploye = await _context.Employes
                .Include(e => e.Utilisateur)
                .FirstOrDefaultAsync(e => e.IdUtilisateur == currentUserId);

            if (currentEmploye == null)
            {
                return NotFound("Employee profile not found for current user");
            }

             var allEmployes = await _context.Employes
                .Include(e => e.Utilisateur)
                .Where(e => e.IdEmploye != currentEmploye.IdEmploye)
                .ToListAsync();

            ViewBag.CurrentEmploye = currentEmploye;
            return View(allEmployes);
        }
         [HttpGet("Messages/Chat/{employeId}")]
        public async Task<IActionResult> Chat(int employeId)
        {
            // Get current user ID from claims
            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Get the employee information for the current user
            var currentEmploye = await _context.Employes
                .Include(e => e.Utilisateur)
                .FirstOrDefaultAsync(e => e.IdUtilisateur == currentUserId);

            if (currentEmploye == null)
            {
                return NotFound("Employee profile not found for current user");
            }

            // Get the target employee
            var targetEmploye = await _context.Employes
                .Include(e => e.Utilisateur)
                .FirstOrDefaultAsync(e => e.IdEmploye == employeId);

            if (targetEmploye == null)
            {
                return NotFound("Target employee not found");
            }

            // Get conversation history
            var messages = await _context.Messages
                .Where(m =>
                    (m.SenderId == currentEmploye.IdEmploye && m.ReceiverId == employeId) ||
                    (m.SenderId == employeId && m.ReceiverId == currentEmploye.IdEmploye))
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            // Get all employees for the sidebar
            var allEmployes = await _context.Employes
                .Include(e => e.Utilisateur)
                .Where(e => e.IdEmploye != currentEmploye.IdEmploye)
                .ToListAsync();

            // Update unread messages to read
            var unreadMessages = messages.Where(m => m.ReceiverId == currentEmploye.IdEmploye && !m.IsRead).ToList();
            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.IsRead = true;
                }
                await _context.SaveChangesAsync();
            }

            ViewBag.CurrentEmploye = currentEmploye;
            ViewBag.TargetEmploye = targetEmploye;
            ViewBag.AllEmployes = allEmployes;
            ViewBag.PusherKey = _configuration["Pusher:AppKey"];
            ViewBag.PusherCluster = _configuration["Pusher:Cluster"];

            return View(messages);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageViewModel model)
        {
            try
            {
                // Get current user ID from claims
                int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Get the employee information for the current user
                var currentEmploye = await _context.Employes
                       .Include(e => e.Utilisateur)
                    .FirstOrDefaultAsync(e => e.IdUtilisateur == currentUserId);

                if (currentEmploye == null)
                {
                    return NotFound("Employee profile not found for current user");
                }

                // Create the message
                var message = new Message
                {
                    SenderId = currentEmploye.IdEmploye,
                    ReceiverId = model.ReceiverId,
                    Content = model.Content,
                    SentAt = DateTime.Now,
                    IsRead = false
                };

                // Save to database
                _context.Messages.Add(message);
                await _context.SaveChangesAsync(); 

                var notification = new Notification
                {
                    IdEmploye = model.ReceiverId,
                    Message = $"You have received a new message from {currentEmploye.NomComplet}",
                    CreatedAt = DateTime.Now,
                    IsRead = false

                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();


                // Return the message immediately without Pusher
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error sending message: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }
        // Add to your controller
      
        // GET: Messages/GetUnreadCount
        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            // Get current user ID from claims
            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Get the employee information for the current user
            var currentEmploye = await _context.Employes
                .FirstOrDefaultAsync(e => e.IdUtilisateur == currentUserId);

            if (currentEmploye == null)
            {
                return NotFound("Employee profile not found for current user");
            }

            // Count unread messages by sender
            var unreadBySender = await _context.Messages
                .Where(m => m.ReceiverId == currentEmploye.IdEmploye && !m.IsRead)
                .GroupBy(m => m.SenderId)
                .Select(g => new { SenderId = g.Key, Count = g.Count() })
                .ToListAsync();

            return Ok(unreadBySender);
        }
    }
}