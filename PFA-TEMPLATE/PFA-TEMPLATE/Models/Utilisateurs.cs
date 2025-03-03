using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PFA_TEMPLATE.viewModels
{
    public class Utilisateur
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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


        [Column(TypeName = "VARCHAR(255)")]
        public string Password { get; set; }


        public string Telephone { get; set; }

        public string Role { get; set; } // e.g., Admin, Employee
    }
}
