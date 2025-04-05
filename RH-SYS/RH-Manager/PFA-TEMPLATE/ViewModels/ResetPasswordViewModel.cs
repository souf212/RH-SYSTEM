using System.ComponentModel.DataAnnotations;

namespace PFA_TEMPLATE.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Le nouveau mot de passe est requis.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "La confirmation du mot de passe est requise.")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Les mots de passe ne correspondent pas.")]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }
    }
}