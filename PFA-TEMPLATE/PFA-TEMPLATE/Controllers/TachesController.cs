using Microsoft.AspNetCore.Mvc;
using PFA_TEMPLATE.Services;
using PFA_TEMPLATE.ViewModels;
namespace PFA_TEMPLATE.Controllers
{
    public class TachesController : Controller
    {
        private readonly ITacheService _tacheService;

        public TachesController(ITacheService tacheService)
        {
            _tacheService = tacheService;
        }

        public IActionResult Basic1()
        {
            var taches = _tacheService.GetAllTaches();
            return View(taches);
        }

        public IActionResult Basic2()
        {
            var loggedInUser = HttpContext.User.Identity.Name?.ToLower();
            if (string.IsNullOrEmpty(loggedInUser))
            {
                return Unauthorized();
            }

            var taches = _tacheService.GetTachesByEmployee(loggedInUser);
            if (!taches.Any())
            {
                ViewBag.Message = "No tasks assigned to you.";
            }

            return View(taches);
        }


        public IActionResult Create()
        {
            ViewData["Employes"] = _tacheService.GetEmployesForDropdown();
            return View(new TachesVM());
        }

        [HttpPost]
        public IActionResult Create(TachesVM tachesVM)
        {
            if (ModelState.IsValid)
            {
                _tacheService.CreateTache(tachesVM);
                return RedirectToAction("Basic1");
            }
            return View(tachesVM);
        }

        public IActionResult Edit(int id)
        {
            var tache = _tacheService.GetTacheById(id);
            if (tache == null) return NotFound();

            ViewData["Employes"] = _tacheService.GetEmployesForDropdown();
            return View(tache);
        }

        [HttpPost]
        public IActionResult Edit(TachesVM tachesVM)
        {
            if (ModelState.IsValid)
            {
                _tacheService.UpdateTache(tachesVM);
                return RedirectToAction("Basic1");
            }
            return View(tachesVM);
        }

        public IActionResult EditStatus(int id)
        {
            var tache = _tacheService.GetTacheById(id);
            if (tache == null) return NotFound();

            return View(tache);
        }

        [HttpPost]
        public IActionResult EditStatus(TachesVM model)
        {
            if (ModelState.IsValid)
            {
                _tacheService.UpdateTacheStatus(model);
                return RedirectToAction("Basic2");
            }
            return View(model);
        }

        public IActionResult Delete(int id)
        {
            var tache = _tacheService.GetTacheById(id);
            if (tache == null) return NotFound();

            return View(tache);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _tacheService.DeleteTache(id);
            return RedirectToAction("Basic1");
        }
    }
}