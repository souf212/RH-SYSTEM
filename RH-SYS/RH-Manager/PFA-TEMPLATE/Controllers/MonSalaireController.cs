using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PFA_TEMPLATE.Data;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace PFA_TEMPLATE.Controllers
{
    public class MonSalaireController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public MonSalaireController(ApplicationDbContext context, IConfiguration config)
        {
            
            _context = context;
            _config = config;
        }

        public async Task<IActionResult> Index()
        {
            var utilisateur = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.Login == User.Identity.Name);

            if (utilisateur == null)
                return NotFound("Utilisateur non connecté");

            var contrat = await _context.Contrats
                .FirstOrDefaultAsync(c => c.IdUtilisateur == utilisateur.Id && c.EtatContrat == "Affecté");

            if (contrat == null)
                return NotFound("Aucun contrat affecté");

            var fiche = await _context.FichesDePaie
                .Where(f => f.IdContrat == contrat.IdContrat)
                .OrderByDescending(f => f.Date)
                .FirstOrDefaultAsync();

            if (fiche == null)
                ViewBag.HasFiche = false;
            else
            {
                ViewBag.HasFiche = true;
                ViewBag.Fiche = fiche;
                ViewBag.Employe = utilisateur.Prenom + " " + utilisateur.Nom;
            }

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Imprimer(int id)
        {
            var fiche = await _context.FichesDePaie
                .Include(f => f.Contrat)
                .ThenInclude(c => c.Utilisateur)
                .FirstOrDefaultAsync(f => f.IdFicheDePaie == id);

            if (fiche == null || fiche.Contrat == null || fiche.Contrat.Utilisateur == null)
                return NotFound("Fiche ou utilisateur introuvable");

            var utilisateur = fiche.Contrat.Utilisateur;

            // 🔢 Heures travaillées
            var pointages = await _context.Pointages
                .Where(p => p.IdEmploye == utilisateur.Id &&
                            p.HeureEntree.Month == fiche.Date.Month &&
                            p.HeureSortie != null)
                .ToListAsync();

            double totalHeures = pointages.Sum(p => (p.HeureSortie.Value - p.HeureEntree).TotalHours);
            var heuresStr = Math.Round(totalHeures, 1).ToString();

            // ⚙️ Appel du script Python
            var pythonPath = _config["PdfGeneration:PythonPath"];
            var scriptPath = _config["PdfGeneration:ScriptPaiePath"];

            var nom = utilisateur.Nom + " " + utilisateur.Prenom;
            var type = fiche.Contrat.TypeContrat;
            var salaire = fiche.SalaireNet.ToString("0.00");
            var mois = fiche.Date.ToString("MMMM yyyy");
            var date = fiche.Date.ToShortDateString();

            var args = $"\"{scriptPath}\" \"{nom}\" \"{type}\" \"{salaire}\" \"{mois}\" \"{date}\" \"{heuresStr}\"";

            var psi = new ProcessStartInfo
            {
                FileName = pythonPath,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            string outputPath;
            using (var process = new Process { StartInfo = psi })
            {
                process.Start();
                outputPath = await process.StandardOutput.ReadLineAsync();
                var errors = await process.StandardError.ReadToEndAsync();
                process.WaitForExit();

                if (string.IsNullOrWhiteSpace(outputPath) || !System.IO.File.Exists(outputPath))
                {
                    return Content("Erreur lors de la génération du PDF 🔥\n" +
                        $"Script Path: {scriptPath}\n" +
                        $"Arguments: {args}\n\n" +
                        $"Erreur Python : {errors}");
                }
            }


            var fileBytes = await System.IO.File.ReadAllBytesAsync(outputPath);
            var fileName = Path.GetFileName(outputPath);

            return File(fileBytes, "application/pdf", fileName);
        }

    }
}
