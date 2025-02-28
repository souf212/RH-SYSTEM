using HR_Management_System.Data;
using HR_Management_System.viewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HR_Management_System.Controllers
{
    public class PlanningController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlanningController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Planning - Admin view (all plannings)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var plannings = await _context.Plannings
                .Include(p => p.Employe)
                .ThenInclude(e => e.Utilisateur)
                .OrderByDescending(p => p.DateDebut)
                .ToListAsync();

            return View(plannings);
        }

        // GET: Planning/EmployeePlanning - Employee view (their own plannings)
        [Authorize(Roles = "User")]
        public async Task<IActionResult> EmployeePlanning()
        {
            // Get current user ID
            var userId = int.Parse(User.FindFirst("UserId").Value);

            // Get employee ID from user ID
            var employe = await _context.Employes
                .FirstOrDefaultAsync(e => e.IdUtilisateur == userId);

            if (employe == null)
            {
                TempData["ErrorMessage"] = "Employé non trouvé.";
                return RedirectToAction("Index", "Home");
            }

            var plannings = await _context.Plannings
                .Where(p => p.IdEmploye == employe.IdEmploye)
                .OrderByDescending(p => p.DateDebut)
                .ToListAsync();

            return View(plannings);
        }

        // GET: Planning/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planning = await _context.Plannings
                .Include(p => p.Employe)
                .ThenInclude(e => e.Utilisateur)
                .FirstOrDefaultAsync(m => m.IdPlanning == id);

            if (planning == null)
            {
                return NotFound();
            }

            return View(planning);
        }

        // GET: Planning/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            // Get all employees for the dropdown
            var employees = await _context.Employes
                .Include(e => e.Utilisateur)
                .ToListAsync();

            ViewBag.Employees = new SelectList(employees, "IdEmploye", "NomComplet");

            return View();
        }

        // POST: Planning/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("IdPlanning,DateDebut,DateFin,Statut,IdEmploye")] Planning planning)
        {
            if (ModelState.IsValid)
            {
                // Validate dates
                if (planning.DateFin < planning.DateDebut)
                {
                    ModelState.AddModelError("DateFin", "La date de fin doit être postérieure à la date de début.");
                    var employees = await _context.Employes
                        .Include(e => e.Utilisateur)
                        .ToListAsync();
                    ViewBag.Employees = new SelectList(employees, "IdEmploye", "NomComplet");
                    return View(planning);
                }

                // Check for overlapping plannings for the same employee
                var overlappingPlannings = await _context.Plannings
                    .Where(p => p.IdEmploye == planning.IdEmploye &&
                           ((p.DateDebut <= planning.DateDebut && p.DateFin >= planning.DateDebut) ||
                            (p.DateDebut <= planning.DateFin && p.DateFin >= planning.DateFin) ||
                            (p.DateDebut >= planning.DateDebut && p.DateFin <= planning.DateFin)))
                    .ToListAsync();

                if (overlappingPlannings.Any())
                {
                    ModelState.AddModelError("", "Il existe déjà un planning pour cet employé qui chevauche les dates sélectionnées.");
                    var employees = await _context.Employes
                        .Include(e => e.Utilisateur)
                        .ToListAsync();
                    ViewBag.Employees = new SelectList(employees, "IdEmploye", "NomComplet");
                    return View(planning);
                }

                _context.Add(planning);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Planning créé avec succès!";
                return RedirectToAction(nameof(Index));
            }

            var employeesList = await _context.Employes
                .Include(e => e.Utilisateur)
                .ToListAsync();
            ViewBag.Employees = new SelectList(employeesList, "IdEmploye", "NomComplet");
            return View(planning);
        }

        // GET: Planning/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planning = await _context.Plannings.FindAsync(id);
            if (planning == null)
            {
                return NotFound();
            }

            var employees = await _context.Employes
                .Include(e => e.Utilisateur)
                .ToListAsync();
            ViewBag.Employees = new SelectList(employees, "IdEmploye", "NomComplet", planning.IdEmploye);

            return View(planning);
        }

        // POST: Planning/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("IdPlanning,DateDebut,DateFin,Statut,IdEmploye")] Planning planning)
        {
            if (id != planning.IdPlanning)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Validate dates
                if (planning.DateFin < planning.DateDebut)
                {
                    ModelState.AddModelError("DateFin", "La date de fin doit être postérieure à la date de début.");
                    var employees = await _context.Employes
                        .Include(e => e.Utilisateur)
                        .ToListAsync();
                    ViewBag.Employees = new SelectList(employees, "IdEmploye", "NomComplet", planning.IdEmploye);
                    return View(planning);
                }

                // Check for overlapping plannings for the same employee (excluding current planning)
                var overlappingPlannings = await _context.Plannings
                    .Where(p => p.IdEmploye == planning.IdEmploye && p.IdPlanning != planning.IdPlanning &&
                           ((p.DateDebut <= planning.DateDebut && p.DateFin >= planning.DateDebut) ||
                            (p.DateDebut <= planning.DateFin && p.DateFin >= planning.DateFin) ||
                            (p.DateDebut >= planning.DateDebut && p.DateFin <= planning.DateFin)))
                    .ToListAsync();

                if (overlappingPlannings.Any())
                {
                    ModelState.AddModelError("", "Il existe déjà un planning pour cet employé qui chevauche les dates sélectionnées.");
                    var employees = await _context.Employes
                        .Include(e => e.Utilisateur)
                        .ToListAsync();
                    ViewBag.Employees = new SelectList(employees, "IdEmploye", "NomComplet", planning.IdEmploye);
                    return View(planning);
                }

                try
                {
                    _context.Update(planning);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Planning modifié avec succès!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlanningExists(planning.IdPlanning))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            var employeesList = await _context.Employes
                .Include(e => e.Utilisateur)
                .ToListAsync();
            ViewBag.Employees = new SelectList(employeesList, "IdEmploye", "NomComplet", planning.IdEmploye);
            return View(planning);
        }

        // GET: Planning/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planning = await _context.Plannings
                .Include(p => p.Employe)
                .ThenInclude(e => e.Utilisateur)
                .FirstOrDefaultAsync(m => m.IdPlanning == id);

            if (planning == null)
            {
                return NotFound();
            }

            return View(planning);
        }

        // POST: Planning/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var planning = await _context.Plannings.FindAsync(id);
            _context.Plannings.Remove(planning);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Planning supprimé avec succès!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Planning/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var planning = await _context.Plannings.FindAsync(id);
            if (planning == null)
            {
                return NotFound();
            }

            planning.Statut = status;
            _context.Update(planning);
            await _context.SaveChangesAsync();
            TempData["Message"] = $"Statut du planning mis à jour: {status}";
            return RedirectToAction(nameof(Index));
        }

        // GET: Planning/Calendar
        public async Task<IActionResult> Calendar()
        {
            // This action will return a calendar view of all plannings
            // Depending on the role, it will show either all plannings (Admin) or only the user's plannings (Employee)

            var plannings = User.IsInRole("Admin")
                ? await _context.Plannings
                    .Include(p => p.Employe)
                    .ThenInclude(e => e.Utilisateur)
                    .ToListAsync()
                : await GetEmployeePlannings();

            return View(plannings);
        }

        private async Task<List<Planning>> GetEmployeePlannings()
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var employe = await _context.Employes
                .FirstOrDefaultAsync(e => e.IdUtilisateur == userId);

            if (employe == null)
            {
                return new List<Planning>();
            }

            return await _context.Plannings
                .Where(p => p.IdEmploye == employe.IdEmploye)
                .ToListAsync();
        }

        private bool PlanningExists(int id)
        {
            return _context.Plannings.Any(e => e.IdPlanning == id);
        }
    }
}