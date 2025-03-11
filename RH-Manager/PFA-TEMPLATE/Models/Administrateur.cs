using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PFA_TEMPLATE.viewModels
{
    public class Administrateur
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdAdmin { get; set; }

        [ForeignKey("Utilisateurs")]
        public int IdUtilisateur { get; set; }
        public virtual Utilisateur Utilisateur { get; set; }

    }
}
