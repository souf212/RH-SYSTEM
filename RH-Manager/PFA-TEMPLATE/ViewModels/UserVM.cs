using System.ComponentModel.DataAnnotations;

namespace PFA_TEMPLATE.ViewModels
{
    public class UserVM
    {

        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nom { get; set; }

        [Required]
        [MaxLength(50)]
        public string Prenom { get; set; }

        [MaxLength(100)]
        public string Adresse { get; set; }

        [Required]
        [MaxLength(20)]
        public string CIN { get; set; }

        [Required]
        [MaxLength(50)]
        public string Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        public string Telephone { get; set; }

        public string Role { get; set; } // e.g., Admin, Employes
    }
}
