using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PFA_TEMPLATE.viewModels;

namespace PFA_TEMPLATE.Models
{
    public class Manager
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdManager { get; set; }

        [ForeignKey("Utilisateur")]
        public int IdUtilisateur { get; set; }
        public virtual Utilisateur Utilisateur { get; set; }

        // New property to track team members
        public virtual ICollection<Employes> TeamMembers { get; set; }
    }
}