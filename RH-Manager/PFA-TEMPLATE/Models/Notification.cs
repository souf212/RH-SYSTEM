using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PFA_TEMPLATE.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        public string Message { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsRead { get; set; } = false;

        [Required]
        public int IdEmploye { get; set; }

        [ForeignKey("IdEmploye")]
        public virtual Employes? Employe { get; set; }

        public int? IdTache { get; set; }

        [ForeignKey("IdTache")]
        public virtual Taches? Tache { get; set; }
        public NotificationType Type { get; set; }
    }
    public enum NotificationType
    {
        TaskExchangeRequest,
        TaskExchangeApproved,
        TaskExchangeRejected,
        TaskExchangeCancelled,
        General,
        TaskAssigned,
        TaskCompleted
    }
}
