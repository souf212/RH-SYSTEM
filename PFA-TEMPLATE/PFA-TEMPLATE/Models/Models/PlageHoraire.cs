namespace HR_Management_System.Models

{
    public class PlageHoraire
    {
        public int Id { get; set; }
        public int EmploiDuTempsId { get; set; }
        public virtual EmploiDuTemps EmploiDuTemps { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public string TypeActivite { get; set; } // Réunion, Formation, Travail, Pause, etc.
        public string Commentaire { get; set; }
    }
}
