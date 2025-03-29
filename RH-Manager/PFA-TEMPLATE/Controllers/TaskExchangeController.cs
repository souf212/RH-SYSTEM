using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PFA_TEMPLATE.Data;
using PFA_TEMPLATE.Models;
using PFA_TEMPLATE.ViewModels;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PFA_TEMPLATE.Controllers
{
    [Authorize(Roles = "Employes")]
    public class TaskExchangeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaskExchangeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: List all task exchange requests
        public async Task<IActionResult> Index()
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var exchanges = await _context.TaskExchanges
                .Include(te => te.RequestorTask)
                .Include(te => te.ReceiverTask)
                .Include(te => te.Requestor)
                .ThenInclude(e => e.Utilisateur)
                .Include(te => te.Receiver)
                .ThenInclude(e => e.Utilisateur)
                .Where(te => te.ReceiverId == currentUserId || te.RequestorId == currentUserId)
                .OrderByDescending(te => te.RequestDate)
                .ToListAsync();

            return View(exchanges);
        }

        // GET: Form to request a task exchange
        public async Task<IActionResult> Create()
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var myTasks = await _context.Taches
                .Where(t => t.IdEmploye == currentUserId && t.Statut != "Completed")
                .ToListAsync();

            var employeesWithTasks = await _context.Employes
                .Include(e => e.Utilisateur)
                .Include(e => e.Taches.Where(t => t.Statut != "Completed"))
                .Where(e => e.IdEmploye != currentUserId)
                .ToListAsync();

            ViewBag.MyTasks = myTasks;
            ViewBag.EmployeesWithTasks = employeesWithTasks;

            return View();
        }

        // POST: Submit a task exchange request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskExchangeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var requestorTask = await _context.Taches
                    .FirstOrDefaultAsync(t => t.IdTaches == model.RequestorTaskId && t.IdEmploye == currentUserId);

                if (requestorTask == null)
                {
                    ModelState.AddModelError("", "Your selected task not found or doesn't belong to you");
                    return View(model);
                }

                var receiver = await _context.Employes
                    .Include(e => e.Taches.Where(t => t.IdTaches == model.ReceiverTaskId))
                    .FirstOrDefaultAsync(e => e.IdEmploye == model.ReceiverId);

                if (receiver == null)
                {
                    ModelState.AddModelError("", "Employee not found");
                    return View(model);
                }

                var receiverTask = receiver.Taches.FirstOrDefault();
                if (receiverTask == null)
                {
                    ModelState.AddModelError("", "The other employee's task not found or doesn't belong to them");
                    return View(model);
                }

                var exchange = new TaskExchange
                {
                    RequestorTaskId = model.RequestorTaskId,
                    ReceiverTaskId = model.ReceiverTaskId,
                    RequestorId = currentUserId,
                    ReceiverId = model.ReceiverId,
                    Reason = model.Reason,
                    Status = TaskExchangeStatus.Pending,
                    RequestDate = DateTime.Now
                };

                _context.Add(exchange);
                await _context.SaveChangesAsync();

                // Create notification for the receiver
                var notification = new Notification
                {
                    IdEmploye = model.ReceiverId,
                    Message = $"You have a new task exchange request from {User.Identity.Name}",
                    CreatedAt = DateTime.Now,
                    IsRead = false, 
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // POST: Approve a task exchange request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var exchange = await _context.TaskExchanges
                .Include(te => te.RequestorTask)
                .Include(te => te.ReceiverTask)
                .Include(te => te.Requestor)
                .FirstOrDefaultAsync(te => te.Id == id && te.ReceiverId == currentUserId && te.Status == TaskExchangeStatus.Pending);

            if (exchange == null)
            {
                return NotFound();
            }

            // Perform the task swap
            var tempEmployeeId = exchange.RequestorTask.IdEmploye;
            exchange.RequestorTask.IdEmploye = exchange.ReceiverTask.IdEmploye;
            exchange.ReceiverTask.IdEmploye = tempEmployeeId;

            exchange.RequestorTask.LastModified = DateTime.Now;
            exchange.ReceiverTask.LastModified = DateTime.Now;

            exchange.Status = TaskExchangeStatus.Approved;
            exchange.ResponseDate = DateTime.Now;

            _context.Update(exchange.RequestorTask);
            _context.Update(exchange.ReceiverTask);
            _context.Update(exchange);
            await _context.SaveChangesAsync();

            // Create notification for the requestor
            var notification = new Notification
            {
                IdEmploye = exchange.RequestorId,
                Message = $"{User.Identity.Name} has approved your task exchange request",
                CreatedAt = DateTime.Now,
                IsRead = false, 
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Reject a task exchange request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var exchange = await _context.TaskExchanges
                .FirstOrDefaultAsync(te => te.Id == id && te.ReceiverId == currentUserId && te.Status == TaskExchangeStatus.Pending);

            if (exchange == null)
            {
                return NotFound();
            }

            exchange.Status = TaskExchangeStatus.Rejected;
            exchange.ResponseDate = DateTime.Now;

            _context.Update(exchange);
            await _context.SaveChangesAsync();

            // Create notification for the requestor
            var notification = new Notification
            {
                IdEmploye = exchange.RequestorId,
                Message = $"{User.Identity.Name} has rejected your task exchange request",
                CreatedAt = DateTime.Now,
                IsRead = false,  
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Cancel a pending request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var exchange = await _context.TaskExchanges
                .FirstOrDefaultAsync(te => te.Id == id && te.RequestorId == currentUserId && te.Status == TaskExchangeStatus.Pending);

            if (exchange == null)
            {
                return NotFound();
            }

            exchange.Status = TaskExchangeStatus.Cancelled;
            exchange.ResponseDate = DateTime.Now;

            _context.Update(exchange);
            await _context.SaveChangesAsync();

            // Create notification for the receiver
            var notification = new Notification
            {
                IdEmploye = exchange.ReceiverId,
                Message = $"{User.Identity.Name} has cancelled the task exchange request",
                CreatedAt = DateTime.Now,
                IsRead = false, 
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}