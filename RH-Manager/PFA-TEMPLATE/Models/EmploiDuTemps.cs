namespace PFA_TEMPLATE.Models
{
    public class EmploiDuTemps
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public virtual Employes Employee { get; set; }
        public DateTime DateGeneration { get; set; }
        public int ContraintesPlanningId { get; set; }
        public double Fitness { get; set; }
        public virtual ContraintesPlanning ContraintesPlanning { get; set; }
        public virtual ICollection<PlageHoraire> PlagesHoraires { get; set; }
    }

}
