using HR_Management_System.Data;
using HR_Management_System.Models;
using HR_Management_System.Services;
using HR_Management_System.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace planning.Controllers
{
    // Controllers/PlanningController.cs
    public class PlanningController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly GenerationEmploiService _generationService;

        public PlanningController(
            ApplicationDbContext context,
            GenerationEmploiService generationService)
        {
            _context = context;
            _generationService = generationService;
        }

        // GET: /Planning
        public async Task<IActionResult> Index()
        {
            var emploisGeneres = await _context.EmploiDuTemps
                .Include(e => e.Employee)
                .ThenInclude(e => e.Utilisateur)
                .Include(e => e.ContraintesPlanning)
                .OrderByDescending(e => e.DateGeneration)
                .ToListAsync();

            return View(emploisGeneres);
        }
        public async Task<IActionResult> Index1()
        {
            var emploisGeneres = await _context.EmploiDuTemps
                .Include(e => e.Employee)
                .ThenInclude(e => e.Utilisateur)
                .Include(e => e.ContraintesPlanning)
                .OrderByDescending(e => e.DateGeneration)
                .ToListAsync();

            return View(emploisGeneres);
        }

        // GET: /Planning/Configuration
        public IActionResult Configuration()
        {
            var model = new ContraintesPlanning
            {
                DateDebut = DateTime.Today,
                DateFin = DateTime.Today.AddDays(14)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Configuration(ContraintesPlanning model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(SelectEmployees), new { id = model.Id });
            }

            // For debugging - check what errors are in ModelState
            foreach (var state in ModelState)
            {
                if (state.Value.Errors.Count > 0)
                {
                    Console.WriteLine($"{state.Key}: {state.Value.Errors[0].ErrorMessage}");
                    // or add them to ViewBag to display
                }
            }

            return View(model);
        }
        public async Task<IActionResult> SelectEmployees(int id)
        {
            var contraintes = await _context.ContraintesPlanning.FindAsync(id);
            if (contraintes == null)
                return NotFound();

            // Charger les employés avec leurs utilisateurs associés
            var employees = await _context.Employes
                .Include(e => e.Utilisateur) // Inclure la propriété Utilisateur
                .ToListAsync();

            // Créer le modèle de vue
            var model = new SelectEmployeesViewModel
            {
                ContraintesPlanningId = id,
                Employees = employees.Select(e => new SelectEmployeeViewModel
                {
                    EmployeeId = e.IdEmploye,
                    Nom = e.Utilisateur?.Nom ?? "Non défini", // Gérer le cas où Utilisateur est null
                    Prenom = e.Utilisateur?.Prenom ?? "Non défini", // Gérer le cas où Utilisateur est null
                    IsSelected = false
                }).ToList()
            };

            return View(model);
        }
        // POST: /Planning/SelectEmployees
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectEmployees(SelectEmployeesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var selectedEmployees = model.Employees
                    .Where(e => e.IsSelected)
                    .Select(e => e.EmployeeId)
                    .ToList();

                foreach (var employeeId in selectedEmployees)
                {
                    await _generationService.GenererEmploiDuTemps(employeeId, model.ContraintesPlanningId);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: /Planning/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var emploiDuTemps = await _context.EmploiDuTemps
                .Include(e => e.Employee)
                 .ThenInclude(e => e.Utilisateur)
                .Include(e => e.ContraintesPlanning)
                .Include(e => e.PlagesHoraires)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (emploiDuTemps == null)
                return NotFound();

            return View(emploiDuTemps);
        }

        // GET: /Planning/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var emploiDuTemps = await _context.EmploiDuTemps
                .Include(e => e.Employee)
                .Include(e => e.ContraintesPlanning)
                .Include(e => e.PlagesHoraires)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (emploiDuTemps == null)
                return NotFound();

            return View(emploiDuTemps);
        }

        // POST: /Planning/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmploiDuTemps model)
        {
            if (id != model.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.EmploiDuTemps.Any(e => e.Id == id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Details), new { id });
            }

            return View(model);
        }

        // POST: /Planning/SaveEvent
        [HttpPost]
        public async Task<IActionResult> SaveEvent(int emploiId, int? plageId, DateTime start, DateTime end, string title, string type)
        {
            try
            {
                if (plageId.HasValue)
                {
                    // Modifier une plage existante
                    var plage = await _context.PlageHoraire.FindAsync(plageId.Value);
                    if (plage == null)
                        return Json(new { success = false, message = "Plage non trouvée" });

                    plage.DateDebut = start;
                    plage.DateFin = end;
                    plage.TypeActivite = type;
                    plage.Commentaire = title;

                    _context.Update(plage);
                }
                else
                {
                    // Créer une nouvelle plage
                    var plage = new PlageHoraire
                    {
                        EmploiDuTempsId = emploiId,
                        DateDebut = start,
                        DateFin = end,
                        TypeActivite = type,
                        Commentaire = title
                    };

                    _context.Add(plage);
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /Planning/DeleteEvent
        [HttpPost]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                var plage = await _context.PlageHoraire.FindAsync(id);
                if (plage == null)
                    return Json(new { success = false, message = "Plage non trouvée" });

                _context.PlageHoraire.Remove(plage);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: /Planning/MonEmploi
        public async Task<IActionResult> MonEmploi(int id)
        {

            var emploiDuTemps = await _context.EmploiDuTemps
                .Include(e => e.Employee)
                 .ThenInclude(e => e.Utilisateur)
                .Include(e => e.ContraintesPlanning)
                .Include(e => e.PlagesHoraires)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (emploiDuTemps == null)
                return NotFound();

            return View(emploiDuTemps);
        }

    }
}
