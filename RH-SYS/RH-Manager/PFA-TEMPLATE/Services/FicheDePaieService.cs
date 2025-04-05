using Microsoft.EntityFrameworkCore;
using PFA_TEMPLATE.Data;
using PFA_TEMPLATE.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PFA_TEMPLATE.Services
{
    public class FicheDePaieService
    {
        private readonly ApplicationDbContext _context;

        public FicheDePaieService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<FicheDePaie?> GenererFicheMensuelleAsync(int idUtilisateur)
        {
            var contrat = await _context.Contrats
                .FirstOrDefaultAsync(c => c.IdUtilisateur == idUtilisateur && c.EtatContrat == "Affecté");

            if (contrat == null) return null;

            var employe = await _context.Employes.FirstOrDefaultAsync(e => e.IdUtilisateur == idUtilisateur);
            if (employe == null) return null;

            var debutMois = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            // ✅ Vérifie si la fiche existe déjà
            var ficheExistante = await _context.FichesDePaie
                .FirstOrDefaultAsync(f => f.IdContrat == contrat.IdContrat &&
                                          f.Date.Year == debutMois.Year &&
                                          f.Date.Month == debutMois.Month);

            if (ficheExistante != null)
                return ficheExistante; // fiche déjà existante, on ne regénère pas

            // 🔍 Calcul total heures
            var finMois = debutMois.AddMonths(1).AddDays(-1);
            var pointages = await _context.Pointages
                .Where(p => p.IdEmploye == employe.IdEmploye &&
                            p.HeureEntree >= debutMois &&
                            p.HeureEntree <= finMois &&
                            p.HeureSortie != null)
                .ToListAsync();

            double totalHeures = pointages.Sum(p => (p.HeureSortie.Value - p.HeureEntree).TotalHours);

            // 💰 Calcul salaire net
            var tauxHoraire = contrat.SalaireDeBase / 160;
            var salaireNet = Math.Round((decimal)totalHeures * tauxHoraire, 2);

            var fiche = new FicheDePaie
            {
                IdContrat = contrat.IdContrat,
                Date = DateTime.Now,
                SalaireNet = salaireNet
            };

            _context.FichesDePaie.Add(fiche);
            await _context.SaveChangesAsync();

            return fiche;
        }

    }
}
