using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PFA_TEMPLATE.Data;
using PFA_TEMPLATE.Models;
using PFA_TEMPLATE.viewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PFA_TEMPLATE.Controllers
{
    [Route("api/pointage")]
    [ApiController]
    public class PointageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PointageController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// ✅ **[POST] Enregistrer le Pointage (Check-in)**
        [HttpPost]
        public async Task<IActionResult> RegisterPointage([FromBody] Pointage pointage)
        {
            if (pointage == null || pointage.IdEmploye <= 0)
            {
                return BadRequest(new { message = "Invalid employee ID or missing data." });
            }

            // ✅ Vérifier si l'employé existe
            var employeExists = await _context.Employes.AnyAsync(e => e.IdEmploye == pointage.IdEmploye);
            if (!employeExists)
            {
                return NotFound(new { message = $"❌ Employee with ID {pointage.IdEmploye} not found." });
            }

            // ✅ Vérifier s'il y a un pointage en attente de sortie
            var lastPointage = await _context.Pointages
                .Where(p => p.IdEmploye == pointage.IdEmploye && p.HeureSortie == null)
                .OrderByDescending(p => p.HeureEntree)
                .FirstOrDefaultAsync();

            if (lastPointage != null)
            {
                // ✅ **MODIFICATION : Temps réduit à 1 minute au lieu de 30 minutes**
                if ((DateTime.Now - lastPointage.HeureEntree).TotalMinutes > 1)
                {
                    lastPointage.HeureSortie = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "✅ Sortie enregistrée avec succès.", lastPointage.IdPointage });
                }
                else
                {
                    return BadRequest(new { message = "⚠ Pointage déjà en cours. Sortie non enregistrée." });
                }
            }

            // ✅ Sinon, créer un nouveau pointage (entrée)
            var newPointage = new Pointage
            {
                IdEmploye = pointage.IdEmploye,
                HeureEntree = DateTime.Now,
                HeureSortie = null
            };

            _context.Pointages.Add(newPointage);
            await _context.SaveChangesAsync();

            return Ok(new { message = "✅ Pointage enregistré avec succès.", newPointage.IdPointage });
        }

        /// ✅ **[PUT] Enregistrer l'Heure de Sortie (Check-out)**
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePointage(int id)
        {
            // ✅ Vérifier si l'employé existe
            var employeExists = await _context.Employes.AnyAsync(e => e.IdEmploye == id);
            if (!employeExists)
            {
                return NotFound(new { message = $"❌ Employee with ID {id} does not exist." });
            }

            // ✅ Trouver le dernier pointage sans HeureSortie
            var pointage = await _context.Pointages
                .Where(p => p.IdEmploye == id && p.HeureSortie == null)
                .OrderByDescending(p => p.HeureEntree)
                .FirstOrDefaultAsync();

            if (pointage == null)
            {
                return NotFound(new { message = "⚠ No active entry found for checkout." });
            }

            pointage.HeureSortie = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok(new { message = "✅ Checkout recorded successfully." });
        }

        /// ✅ **[GET] Récupérer les Pointages d'Aujourd'hui**
        [HttpGet("today")]
        public async Task<IActionResult> GetTodayPointage()
        {
            var today = DateTime.Today;
            var pointages = await _context.Pointages
                .Where(p => p.HeureEntree.Date == today)
                .Select(p => new
                {
                    p.IdEmploye,
                    p.HeureEntree,
                    HeureSortie = p.HeureSortie.HasValue ? p.HeureSortie.Value.ToString("yyyy-MM-dd HH:mm:ss") : "En cours"
                })
                .ToListAsync();

            if (!pointages.Any())
            {
                return NotFound(new { message = "⚠ No attendance records found for today." });
            }

            return Ok(pointages);
        }

        /// ✅ **[GET] Récupérer Tous les Pointages**
        [HttpGet]
        public async Task<IActionResult> GetAllPointages()
        {
            var pointages = await _context.Pointages
                .Select(p => new
                {
                    p.IdPointage,
                    p.IdEmploye,
                    p.HeureEntree,
                    HeureSortie = p.HeureSortie.HasValue ? p.HeureSortie.Value.ToString("yyyy-MM-dd HH:mm:ss") : "En cours"
                })
                .ToListAsync();

            if (!pointages.Any())
            {
                return NotFound(new { message = "⚠ No pointage records found." });
            }

            return Ok(pointages);
        }
    }
}
