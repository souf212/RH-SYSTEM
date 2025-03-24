using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PFA_TEMPLATE.Data;
using PFA_TEMPLATE.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PFA_TEMPLATE.Controllers
{
    [Route("api/employes")]
    [ApiController]
    public class EmployesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmployesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// ✅ **[GET] Fetch Employee by `IdEmploye`**
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmploye(int id)
        {
            var employe = await _context.Employes
                .Include(e => e.Utilisateur) // ✅ Ensure `Utilisateur` data is included
                .FirstOrDefaultAsync(e => e.IdEmploye == id);

            if (employe == null)
            {
                return NotFound(new { message = $"❌ Employee with ID {id} not found." });
            }

            return Ok(new
            {
                employe.IdEmploye,
                employe.IdUtilisateur,
                employe.NomComplet, // ✅ Uses computed property
                employe.Utilisateur?.Nom,
                employe.Utilisateur?.Prenom
            });
        }
    }
}
