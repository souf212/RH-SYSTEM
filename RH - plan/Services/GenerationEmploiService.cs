using HR_Management_System.Data;
using HR_Management_System.Models;
namespace HR_Management_System.Services
{
    // Services/GenerationEmploiService.cs
    public class GenerationEmploiService
    {
        private readonly ApplicationDbContext _context;
        private readonly Random _random = new Random();
        private const int TAILLE_POPULATION = 50;
        private const int NOMBRE_GENERATIONS = 100;
        private const double PROBABILITE_MUTATION = 0.1;
        private const double PROBABILITE_CROISEMENT = 0.8;

        public GenerationEmploiService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EmploiDuTemps> GenererEmploiDuTemps(int employeeId, int contraintesPlanningId)
        {
            var employee = await _context.Employes.FindAsync(employeeId);
            var contraintes = await _context.ContraintesPlanning.FindAsync(contraintesPlanningId);

            if (employee == null || contraintes == null)
                throw new ArgumentException("Employé ou contraintes introuvables");

            // 1. Initialiser la population
            var population = InitialiserPopulation(employee, contraintes);

            // 2. Évoluer la population à travers plusieurs générations
            for (int g = 0; g < NOMBRE_GENERATIONS; g++)
            {
                // Évaluer la fitness de chaque individu
                population = EvaluerPopulation(population, contraintes);

                // Sélectionner les meilleurs individus
                var selection = SelectionnerMeilleurs(population);

                // Créer une nouvelle génération
                var nouvellePopulation = new List<EmploiDuTemps>();

                // Élitisme: conserver le meilleur individu
                nouvellePopulation.Add(selection[0]);

                // Croisement et mutation
                while (nouvellePopulation.Count < TAILLE_POPULATION)
                {
                    var parent1 = SelectionTournoi(selection);
                    var parent2 = SelectionTournoi(selection);

                    var enfant = _random.NextDouble() < PROBABILITE_CROISEMENT
                        ? Croisement(parent1, parent2)
                        : CloneEmploi(parent1);

                    if (_random.NextDouble() < PROBABILITE_MUTATION)
                        Mutation(enfant, contraintes);

                    nouvellePopulation.Add(enfant);
                }

                population = nouvellePopulation;
            }

            // Retourner le meilleur emploi du temps
            var meilleurEmploi = EvaluerPopulation(population, contraintes)[0];

            // Sauvegarder dans la base de données
            meilleurEmploi.DateGeneration = DateTime.Now;
            meilleurEmploi.EmployeeId = employeeId;
            meilleurEmploi.ContraintesPlanningId = contraintesPlanningId;

            _context.EmploiDuTemps.Add(meilleurEmploi);
            await _context.SaveChangesAsync();

            return meilleurEmploi;
        }
        private string ObtenirCommentaireSelonHeure(int heure)
        {
            if (heure >= 8 && heure < 10)
                return "Taches prioritaires";
            else if (heure >= 10 && heure < 12)
                return "Traitement des projets";
            else if (heure >= 12 && heure < 14)
                return "Finalisation avant pause";
            else if (heure >= 14 && heure < 16)
                return "Coordination d equipe";
            else if (heure >= 16 && heure < 18)
                return "Reunion avec equipe";
            else
                return "Horaire special";
        }
        private List<EmploiDuTemps> InitialiserPopulation(Employes employee, ContraintesPlanning contraintes)
        {
            var population = new List<EmploiDuTemps>();

            for (int i = 0; i < TAILLE_POPULATION; i++)
            {
                var emploi = new EmploiDuTemps
                {
                    EmployeeId = employee.IdEmploye,
                    ContraintesPlanningId = contraintes.Id,
                    PlagesHoraires = new List<PlageHoraire>()
                };

                // Générer des plages horaires aléatoires
                var currentDate = contraintes.DateDebut;
                while (currentDate <= contraintes.DateFin)
                {
                    // Sauter les weekends si nécessaire
                    if (!contraintes.WeekEndInclus && (currentDate.DayOfWeek == DayOfWeek.Saturday || currentDate.DayOfWeek == DayOfWeek.Sunday))
                    {
                        currentDate = currentDate.AddDays(1);
                        continue;
                    }

                    // Déterminer les heures de travail pour la journée
                    var heureDebut = contraintes.HeureDebutJournee;
                    var heuresFin = contraintes.HeureFinJournee;
                    var dureeJournee = _random.Next(contraintes.DureeMinimaleJournaliere, contraintes.DureeMaximaleJournaliere + 1);

                    // Créer des créneaux de travail
                    int heuresRestantes = dureeJournee;
                    int heureActuelle = heureDebut;

                    while (heuresRestantes > 0 && heureActuelle < heuresFin)
                    {
                        // Durée de ce créneau
                        int duree = Math.Min(_random.Next(2, 4), heuresRestantes);

                        // Ajouter le créneau
                        emploi.PlagesHoraires.Add(new PlageHoraire
                        {
                            DateDebut = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, heureActuelle, 0, 0),
                            DateFin = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, heureActuelle + duree, 0, 0),
                            TypeActivite = "Travail",
                            Commentaire = ObtenirCommentaireSelonHeure(heureActuelle)
                        });

                        heuresRestantes -= duree;
                        heureActuelle += duree;
                        bool pauseMatinPrise = false;
                        // Ajouter une pause si nécessaire
                        if (heuresRestantes > 0 && _random.NextDouble() < 0.7)
                        {
                            int dureePause = Math.Min(contraintes.PauseMinimum, 1);
                            string commentairePause;

                            if (heureActuelle < 12)
                            {
                                commentairePause = "Pause cafe";
                                pauseMatinPrise = true;
                            }
                            else if (heureActuelle >= 12 && heureActuelle < 14)
                            {
                                commentairePause = "Pause dejeuner";
                            }
                            else
                            {
                                commentairePause = "Pause recuperation";
                            }

                            emploi.PlagesHoraires.Add(new PlageHoraire
                            {
                                DateDebut = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, heureActuelle, 0, 0),
                                DateFin = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, heureActuelle + dureePause, 0, 0),
                                TypeActivite = "Pause",
                                Commentaire = commentairePause
                            });

                            heureActuelle += dureePause;
                        }

                    }

                    currentDate = currentDate.AddDays(1);
                }

                population.Add(emploi);
            }

            return population;
        }

        private List<EmploiDuTemps> EvaluerPopulation(List<EmploiDuTemps> population, ContraintesPlanning contraintes)
        {
            foreach (var emploi in population)
            {
                emploi.Fitness = CalculerFitness(emploi, contraintes);
            }

            return population.OrderByDescending(e => e.Fitness).ToList();
        }

        private double CalculerFitness(EmploiDuTemps emploi, ContraintesPlanning contraintes)
        {
            double score = 100.0;

            // Vérifier la durée totale de travail
            var plagesTravail = emploi.PlagesHoraires.Where(p => p.TypeActivite == "Travail").ToList();
            var dureeParJour = plagesTravail
                .GroupBy(p => p.DateDebut.Date)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(p => (p.DateFin - p.DateDebut).TotalHours)
                );

            // Pénaliser les journées trop courtes ou trop longues
            foreach (var jour in dureeParJour)
            {
                if (jour.Value < contraintes.DureeMinimaleJournaliere)
                    score -= 5 * (contraintes.DureeMinimaleJournaliere - jour.Value);
                if (jour.Value > contraintes.DureeMaximaleJournaliere)
                    score -= 10 * (jour.Value - contraintes.DureeMaximaleJournaliere);
            }

            // Vérifier les pauses
            var joursSansPause = plagesTravail
                .GroupBy(p => p.DateDebut.Date)
                .Where(g => g.Sum(p => (p.DateFin - p.DateDebut).TotalHours) > 5)
                .Where(g => !emploi.PlagesHoraires.Any(p => p.TypeActivite == "Pause" && p.DateDebut.Date == g.Key))
                .Count();

            score -= joursSansPause * 15;

            // Vérifier les chevauchements
            var plagesTriees = emploi.PlagesHoraires.OrderBy(p => p.DateDebut).ToList();
            for (int i = 0; i < plagesTriees.Count - 1; i++)
            {
                if (plagesTriees[i].DateFin > plagesTriees[i + 1].DateDebut)
                    score -= 20;
            }

            return Math.Max(0, score);
        }

        private List<EmploiDuTemps> SelectionnerMeilleurs(List<EmploiDuTemps> population)
        {
            // Sélectionner les 40% meilleurs
            return population.Take((int)(TAILLE_POPULATION * 0.4)).ToList();
        }

        private EmploiDuTemps SelectionTournoi(List<EmploiDuTemps> selection)
        {
            // Sélectionner 3 individus au hasard et prendre le meilleur
            var candidats = new List<EmploiDuTemps>();
            for (int i = 0; i < 3; i++)
            {
                int index = _random.Next(selection.Count);
                candidats.Add(selection[index]);
            }

            return candidats.OrderByDescending(e => e.Fitness).First();
        }

        private EmploiDuTemps Croisement(EmploiDuTemps parent1, EmploiDuTemps parent2)
        {
            var enfant = new EmploiDuTemps
            {
                EmployeeId = parent1.EmployeeId,
                ContraintesPlanningId = parent1.ContraintesPlanningId,
                PlagesHoraires = new List<PlageHoraire>()
            };

            // Point de croisement basé sur la date
            var plagesParent1 = parent1.PlagesHoraires.OrderBy(p => p.DateDebut).ToList();
            var plagesParent2 = parent2.PlagesHoraires.OrderBy(p => p.DateDebut).ToList();

            if (plagesParent1.Count > 0 && plagesParent2.Count > 0)
            {
                var datesMilieu = plagesParent1.Select(p => p.DateDebut.Date).Distinct().ToList();
                var pointCroisement = datesMilieu.Count > 1 ? datesMilieu[_random.Next(1, datesMilieu.Count)] : DateTime.MaxValue;

                // Prendre les plages du parent1 avant le point de croisement
                foreach (var plage in plagesParent1.Where(p => p.DateDebut.Date < pointCroisement))
                {
                    enfant.PlagesHoraires.Add(new PlageHoraire
                    {
                        DateDebut = plage.DateDebut,
                        DateFin = plage.DateFin,
                        TypeActivite = plage.TypeActivite,
                        Commentaire = plage.Commentaire
                    });
                }

                // Prendre les plages du parent2 après le point de croisement
                foreach (var plage in plagesParent2.Where(p => p.DateDebut.Date >= pointCroisement))
                {
                    enfant.PlagesHoraires.Add(new PlageHoraire
                    {
                        DateDebut = plage.DateDebut,
                        DateFin = plage.DateFin,
                        TypeActivite = plage.TypeActivite,
                        Commentaire = plage.Commentaire
                    });
                }
            }

            return enfant;
        }

        private void Mutation(EmploiDuTemps emploi, ContraintesPlanning contraintes)
        {
            if (emploi.PlagesHoraires.Count == 0)
                return;

            // Plusieurs types de mutations possibles
            int typeMutation = _random.Next(3);

            switch (typeMutation)
            {
                case 0:
                    // Modifier l'heure de début d'une plage
                    var plageAModifier = emploi.PlagesHoraires.ElementAt(_random.Next(emploi.PlagesHoraires.Count));
                    var duree = (plageAModifier.DateFin - plageAModifier.DateDebut).TotalHours;
                    var nouvelleHeure = _random.Next(contraintes.HeureDebutJournee, contraintes.HeureFinJournee - (int)duree);

                    plageAModifier.DateDebut = new DateTime(
                        plageAModifier.DateDebut.Year,
                        plageAModifier.DateDebut.Month,
                        plageAModifier.DateDebut.Day,
                        nouvelleHeure,
                        0,
                        0);
                    plageAModifier.DateFin = plageAModifier.DateDebut.AddHours(duree);
                    break;

                case 1:
                    // Ajouter une nouvelle plage
                    var dateDisponible = CalculerDateDisponible(emploi, contraintes);
                    if (dateDisponible.HasValue)
                    {
                        var heureDebut = _random.Next(contraintes.HeureDebutJournee, contraintes.HeureFinJournee - 1);
                        var dureeCreneau = _random.Next(1, 3);

                        emploi.PlagesHoraires.Add(new PlageHoraire
                        {
                            DateDebut = new DateTime(dateDisponible.Value.Year, dateDisponible.Value.Month, dateDisponible.Value.Day, heureDebut, 0, 0),
                            DateFin = new DateTime(dateDisponible.Value.Year, dateDisponible.Value.Month, dateDisponible.Value.Day, heureDebut + dureeCreneau, 0, 0),
                            TypeActivite = _random.NextDouble() < 0.8 ? "Travail" : "Pause",
                            Commentaire = "Mutation"
                        });
                    }
                    break;

                case 2:
                    // Supprimer une plage si plus de 5 plages
                    if (emploi.PlagesHoraires.Count > 5)
                    {
                        var plageASupprimer = emploi.PlagesHoraires.ElementAt(_random.Next(emploi.PlagesHoraires.Count));
                        emploi.PlagesHoraires.Remove(plageASupprimer);
                    }
                    break;
            }
        }

        private DateTime? CalculerDateDisponible(EmploiDuTemps emploi, ContraintesPlanning contraintes)
        {
            var datesUtilisees = emploi.PlagesHoraires.Select(p => p.DateDebut.Date).Distinct().ToList();
            var datesPossibles = new List<DateTime>();

            for (var date = contraintes.DateDebut; date <= contraintes.DateFin; date = date.AddDays(1))
            {
                if (!contraintes.WeekEndInclus && (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday))
                    continue;

                if (!datesUtilisees.Contains(date))
                    datesPossibles.Add(date);
            }

            if (datesPossibles.Count > 0)
                return datesPossibles[_random.Next(datesPossibles.Count)];

            return null;
        }

        private EmploiDuTemps CloneEmploi(EmploiDuTemps source)
        {
            var clone = new EmploiDuTemps
            {
                EmployeeId = source.EmployeeId,
                ContraintesPlanningId = source.ContraintesPlanningId,
                PlagesHoraires = new List<PlageHoraire>()
            };

            foreach (var plage in source.PlagesHoraires)
            {
                clone.PlagesHoraires.Add(new PlageHoraire
                {
                    DateDebut = plage.DateDebut,
                    DateFin = plage.DateFin,
                    TypeActivite = plage.TypeActivite,
                    Commentaire = plage.Commentaire
                });
            }

            return clone;
        }
    }
}
