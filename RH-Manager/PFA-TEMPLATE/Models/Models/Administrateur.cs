using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR_Management_System.viewModels
{
    public class Administrateur
    {
        [Key]
        public int IdAdmin { get; set; }

        [ForeignKey("Utilisateurs")]
        public int IdUtilisateur { get; set; }
        public virtual Utilisateur Utilisateur { get; set; }

    }
}
