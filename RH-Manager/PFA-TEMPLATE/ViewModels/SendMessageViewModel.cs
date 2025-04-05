using System.ComponentModel.DataAnnotations;

namespace PFA_TEMPLATE.ViewModels
{
    public class SendMessageViewModel
    {
        [Required]
        public int ReceiverId { get; set; }

        [Required]
        public string Content { get; set; }
    }
}