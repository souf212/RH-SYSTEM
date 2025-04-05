// Controllers/ContratsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PFA_TEMPLATE.Data;
using PFA_TEMPLATE.Models;
using PFA_TEMPLATE.ViewModels;
using System.Diagnostics;
using System.IO;
using static Org.BouncyCastle.Math.EC.ECCurve;

public class ContratsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config; // 👈 nouveau

    public ContratsController(ApplicationDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config; // 👈 injecte la config

    }
    public IActionResult Index()
    {
        var contrats = _context.Contrats
            .Include(c => c.Utilisateur)
            .OrderByDescending(c => c.DateDebut)
            .ToList();

        return View(contrats);
    }


    // GET: Contrats/Create
    public IActionResult Create()
    {
        ViewBag.Utilisateurs = _context.Utilisateurs
            .Select(u => new { u.Id, NomComplet = u.Prenom + " " + u.Nom })
            .ToList();

        return View();
    }

    // POST: Contrats/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ContratVM vm)
    {
        if (ModelState.IsValid)
        {
            var contrat = new Contrat
            {
                SalaireDeBase = vm.SalaireDeBase,
                DateDebut = vm.DateDebut,
                DateFin = vm.DateFin,
                TypeContrat = vm.TypeContrat,
                EtatContrat = "En attente",
                IdUtilisateur = null
            };

            _context.Contrats.Add(contrat);
            await _context.SaveChangesAsync();

            // ✅ Redirection directe vers l'affectation
            return RedirectToAction("Affecter", new { id = contrat.IdContrat });
        }

        return View(vm);
    }

    // GET: Contrats/Affecter/5
    public IActionResult Affecter(int id)
    {
        var contrat = _context.Contrats.FirstOrDefault(c => c.IdContrat == id && c.IdUtilisateur == null);
        if (contrat == null)
            return NotFound();

        ViewBag.Utilisateurs = _context.Utilisateurs
            .Select(u => new { u.Id, NomComplet = u.Prenom + " " + u.Nom })
            .ToList();

        var vm = new AffectationContratVM
        {
            ContratId = id
        };

        return View(vm);
    }

    // POST: Contrats/Affecter
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Affecter(AffectationContratVM vm)
    {
        var contrat = await _context.Contrats.FindAsync(vm.ContratId);
        if (contrat == null)
            return NotFound();

        contrat.IdUtilisateur = vm.UtilisateurId;
        contrat.EtatContrat = "Affecté";
        await _context.SaveChangesAsync();

        TempData["success"] = "Contrat affecté avec succès.";
        return RedirectToAction("Index");
    }
    // GET: Contrats/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var contrat = await _context.Contrats
            .Include(c => c.Utilisateur)
            .FirstOrDefaultAsync(c => c.IdContrat == id);

        if (contrat == null)
            return NotFound();

        return View(contrat); // 🔗 va vers Views/Contrats/Details.cshtml
    }
    // GET: Contrats/Imprimer/5
    public async Task<IActionResult> Imprimer(int id)
    {
        var contrat = await _context.Contrats
            .Include(c => c.Utilisateur)
            .FirstOrDefaultAsync(c => c.IdContrat == id);

        if (contrat == null)
            return NotFound();

        return View("Imprimer", contrat); // 🔗 va vers Views/Contrats/Imprimer.cshtml
    }
    [HttpGet]
    public async Task<IActionResult> GenererPdf(int id)
    {
        var contrat = await _context.Contrats
            .Include(c => c.Utilisateur)
            .FirstOrDefaultAsync(c => c.IdContrat == id);

        if (contrat == null || contrat.Utilisateur == null)
            return NotFound("Contrat ou utilisateur introuvable");

        // 🧠 Récupérer les données
        var nom = contrat.Utilisateur.Nom + " " + contrat.Utilisateur.Prenom;
        var email = contrat.Utilisateur.Email;
        var cin = contrat.Utilisateur.CIN;
        var typeContrat = contrat.TypeContrat;
        var dateDebut = contrat.DateDebut.ToString("dd/MM/yyyy");
        var dateFin = contrat.DateFin?.ToString("dd/MM/yyyy") ?? "Non défini";
        var salaire = contrat.SalaireDeBase.ToString();

        // 📁 Chemins
        var pythonPath = _config["PdfGeneration:PythonPath"];
        var scriptPath = _config["PdfGeneration:ScriptPath"];

        var arguments = $"\"{scriptPath}\" \"{nom}\" \"{email}\" \"{cin}\" \"{typeContrat}\" \"{dateDebut}\" \"{dateFin}\" \"{salaire}\"";

        var psi = new ProcessStartInfo
        {
            FileName = pythonPath,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        string pdfPath;

        using (var process = new Process { StartInfo = psi })
        {
            process.Start();

            pdfPath = await process.StandardOutput.ReadLineAsync();
            var errors = await process.StandardError.ReadToEndAsync();

            process.WaitForExit();

            if (process.ExitCode != 0 || !System.IO.File.Exists(pdfPath))
            {
                return Content("Erreur lors de la génération du PDF : " + errors);
            }
        }

        // 📨 Télécharger le PDF généré
        var fileBytes = await System.IO.File.ReadAllBytesAsync(pdfPath);
        var fileName = Path.GetFileName(pdfPath);

        return File(fileBytes, "application/pdf", fileName);
    }


}
