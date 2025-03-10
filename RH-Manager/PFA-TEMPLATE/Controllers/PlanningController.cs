using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PFA_TEMPLATE.Data;
using PFA_TEMPLATE.Mappers;
using PFA_TEMPLATE.Models;
using PFA_TEMPLATE.Repositories;
using PFA_TEMPLATE.Services;
using PFA_TEMPLATE.ViewModels;

namespace planning.Controllers
{
    public class PlanningController : Controller
    {
        private readonly IPlanningRepository _repository;

        public PlanningController(IPlanningRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Retrieve all planning entries with related employee data
            var plannings = _repository.GetAllWithDetails();

            // Map to PlanningViewModel using the mapper
            var planningViewModels = PlanningMapper.ToViewModels(plannings);

            return View(planningViewModels);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new PlanningViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PlanningViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Map ViewModel to Entity
            var planning = PlanningMapper.ToEntity(model);

            await _repository.AddAsync(planning);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var planning = await _repository.GetByIdAsync(id);
            if (planning == null)
            {
                return NotFound();
            }

            // Map Entity to ViewModel
            var viewModel = PlanningMapper.ToViewModel(planning);
            return View("Create", viewModel); // Use the same "Create" view for editing
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PlanningViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", model);
            }

            // Map ViewModel to Entity
            var planning = PlanningMapper.ToEntity(model);

            await _repository.UpdateAsync(planning);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var planning = await _repository.GetByIdAsync(id);
            if (planning == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(planning);
            return RedirectToAction(nameof(Index));
        }
    }
}

