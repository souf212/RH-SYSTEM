using System.ComponentModel.DataAnnotations;

namespace PFA_TEMPLATE.ViewModels
{
    public class UserVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est requis")]
        [StringLength(50, ErrorMessage = "Le nom ne peut pas dépasser 50 caractères")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "Le prénom est requis")]
        [StringLength(50, ErrorMessage = "Le prénom ne peut pas dépasser 50 caractères")]
        public string Prenom { get; set; }

        [Required(ErrorMessage = "L'adresse est requise")]
        [StringLength(100, ErrorMessage = "L'adresse ne peut pas dépasser 100 caractères")]
        public string Adresse { get; set; }

        [Required(ErrorMessage = "Le CIN est requis")]
        [StringLength(20, ErrorMessage = "Le CIN ne peut pas dépasser 20 caractères")]
        public string CIN { get; set; }

        [Required(ErrorMessage = "Le téléphone est requis")]
        [Phone(ErrorMessage = "Numéro de téléphone invalide")]
        public string Telephone { get; set; }

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        [StringLength(100, ErrorMessage = "L'email ne peut pas dépasser 100 caractères")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le login est requis")]
        [StringLength(50, ErrorMessage = "Le login ne peut pas dépasser 50 caractères")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Le mot de passe est requis")]
        [StringLength(255, MinimumLength = 8, ErrorMessage = "Le mot de passe doit avoir entre 8 et 255 caractères")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Le rôle est requis")]
        [StringLength(50, ErrorMessage = "Le rôle ne peut pas dépasser 50 caractères")]
        public string Role { get; set; }
    }
}