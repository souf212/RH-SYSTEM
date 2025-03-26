using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PFA_TEMPLATE.Models
{
    public class TaskExchange
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RequestorTaskId { get; set; }

        [Required]
        public int ReceiverTaskId { get; set; }

        [Required]
        public int RequestorId { get; set; }

        [Required]
        public int ReceiverId { get; set; }

        [Required]
        public string Reason { get; set; }

        [Required]
        public TaskExchangeStatus Status { get; set; } = TaskExchangeStatus.Pending;

        [Required]
        public DateTime RequestDate { get; set; } = DateTime.Now;

        public DateTime? ResponseDate { get; set; }

        // Navigation properties
        [ForeignKey("RequestorTaskId")]
        public virtual Taches RequestorTask { get; set; }

        [ForeignKey("ReceiverTaskId")]
        public virtual Taches ReceiverTask { get; set; }

        [ForeignKey("RequestorId")]
        public virtual Employes Requestor { get; set; }

        [ForeignKey("ReceiverId")]
        public virtual Employes Receiver { get; set; }
    }

    public enum TaskExchangeStatus
    {
        Pending,
        Approved,
        Rejected,
        Cancelled
    }
}