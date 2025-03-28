using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PFA_TEMPLATE.Models
{
    public class TaskExchangeViewModel
    {
        [Required(ErrorMessage = "Your Task to Exchange is required")]
        [Display(Name = "Your Task to Exchange")]
        public int RequestorTaskId { get; set; }

        [Required(ErrorMessage = "Employee to Exchange With is required")]
        [Display(Name = "Employee to Exchange With")]
        public int ReceiverId { get; set; }

        [Required(ErrorMessage = "Their Task to Exchange is required")]
        [Display(Name = "Their Task to Exchange")]
        public int ReceiverTaskId { get; set; }

        [Required(ErrorMessage = "Reason for Exchange is required")]
        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
        [Display(Name = "Reason for Exchange")]
        public string Reason { get; set; }

    }
}