using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PFA_TEMPLATE.Models

{
    public class ContraintesPlanning
    {
        [Key] // Marks this as the primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public int HeureDebutJournee { get; set; } = 8;
        public int HeureFinJournee { get; set; } = 17;
        public bool WeekEndInclus { get; set; } = false;
        public int DureeMaximaleJournaliere { get; set; } = 8;
        public int DureeMinimaleJournaliere { get; set; } = 4;
        public int PauseMinimum { get; set; } = 1;
        public string JoursFeries { get; set; } // Format JSON des dates
    }
}
