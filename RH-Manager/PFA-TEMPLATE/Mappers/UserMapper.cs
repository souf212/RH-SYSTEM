using PFA_TEMPLATE.viewModels;
using PFA_TEMPLATE.ViewModels;
using PFA_TEMPLATE.Models;

namespace PFA_TEMPLATE.Mappers
{
    public static class UserMapper
    {
        // Map Utilisateur (Model) to UserVM (ViewModel)
        public static UserVM MapToUserVM(Utilisateur user)
        {
            if (user == null)
                return null;

            return new UserVM
            {
                Id = user.Id,
                Nom = user.Nom,
                Prenom = user.Prenom,
                Adresse = user.Adresse,
                CIN = user.CIN,
                Email = user.Email, // Added this line
                Login = user.Login,
                Password = user.Password,
                Telephone = user.Telephone,
                Role = user.Role
            };
        }

        // Map UserVM (ViewModel) to Utilisateur (Model)
        public static Utilisateur MapToUtilisateur(UserVM userVM)
        {
            if (userVM == null)
                return null;

            return new Utilisateur
            {
                Id = userVM.Id,
                Nom = userVM.Nom,
                Prenom = userVM.Prenom,
                Adresse = userVM.Adresse,
                CIN = userVM.CIN,
                Email = userVM.Email ?? throw new ArgumentNullException(nameof(userVM.Email)), // Added with validation
                Login = userVM.Login,
                Password = userVM.Password,
                Telephone = userVM.Telephone,
                Role = userVM.Role
            };
        }
    }
}