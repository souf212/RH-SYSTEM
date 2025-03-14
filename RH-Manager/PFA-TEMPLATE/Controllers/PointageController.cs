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

        /// ✅ **[POST] Register Employee Attendance (Check-in)**
        [HttpPost]
        public async Task<IActionResult> RegisterPointage([FromBody] Pointage pointage)
        {
            if (pointage == null || pointage.IdEmploye <= 0)
            {
                return BadRequest(new { message = "Invalid employee ID" });
            }

            pointage.HeureEntree = DateTime.Now;  // Set current time as entry time
            pointage.HeureSortie = null;          // Initialize sortie as null

            _context.Pointages.Add(pointage);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Pointage enregistré avec succès." });
        }

        /// ✅ **[PUT] Update Employee Checkout Time (Check-out)**
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePointage(int id)
        {
            var pointage = await _context.Pointages
                .Where(p => p.IdEmploye == id && p.HeureSortie == null)
                .OrderByDescending(p => p.HeureEntree)
                .FirstOrDefaultAsync();

            if (pointage == null)
            {
                return NotFound(new { message = "No active entry found for checkout." });
            }

            pointage.HeureSortie = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Sortie enregistrée avec succès." });
        }

        /// ✅ **[GET] Retrieve Today's Attendance for All Employees**
        [HttpGet("today")]
        public IActionResult GetTodayPointage()
        {
            var today = DateTime.Today;
            var pointages = _context.Pointages
                .Where(p => p.HeureEntree.Date == today)
                .Include(p => p.Employe)
                .Select(p => new
                {
                    p.IdEmploye,
                    Employe = p.Employe != null ? p.Employe.Nom : "Nom non disponible", // 🔥 Fix Null Reference
                    p.HeureEntree,
                    HeureSortie = p.HeureSortie.HasValue ? p.HeureSortie.Value.ToString("yyyy-MM-dd HH:mm:ss") : "En cours"
                })
                .ToList();

            return Ok(pointages);
        }


        /// ✅ **[GET] Retrieve Attendance History for a Specific Employee**
        /// ✅ **[GET] Retrieve Attendance History for a Specific Employee**
        [HttpGet("history/{id}")]
        public IActionResult GetEmployeePointage(int id)
        {
            var historique = _context.Pointages
                .Where(p => p.IdEmploye == id)
                .OrderByDescending(p => p.HeureEntree)
                .Select(p => new
                {
                    p.HeureEntree,
                    HeureSortie = p.HeureSortie.HasValue ? p.HeureSortie.Value.ToString("yyyy-MM-dd HH:mm:ss") : "En cours",
                    Statut = p.HeureSortie == null ? "Présent" : "Sorti"
                })
                .ToList();

            if (!historique.Any())
            {
                return NotFound(new { message = "No pointage history found for this employee." });
            }

            return Ok(historique);
        }


        /// ✅ **[GET] Map `IdUtilisateur` to `IdEmploye`**
        [HttpGet("map_user_to_employee/{idUtilisateur}")]
        public IActionResult MapUserToEmployee(int idUtilisateur)
        {
            var employe = _context.Employes
                .Where(e => e.IdUtilisateur == idUtilisateur)
                .Select(e => new { e.IdEmploye })
                .FirstOrDefault();

            if (employe == null)
            {
                return NotFound(new { message = $"❌ No employee found with IdUtilisateur = {idUtilisateur}" });
            }

            return Ok(employe);
        }

        /// ✅ **[GET] Map `IdEmploye` to `IdUtilisateur`**
        [HttpGet("map_employee_to_user/{idEmploye}")]
        public IActionResult MapEmployeeToUser(int idEmploye)
        {
            var utilisateur = _context.Employes
                .Where(e => e.IdEmploye == idEmploye)
                .Select(e => new { e.IdUtilisateur })
                .FirstOrDefault();

            if (utilisateur == null)
            {
                return NotFound(new { message = $"❌ No user found for IdEmploye = {idEmploye}" });
            }

            return Ok(utilisateur);
        }
    }
}
