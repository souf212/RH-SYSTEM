using HR_Management_System.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Conges
{
    [Key]
    public int IdConges { get; set; }

    [Required]
    public string Motif { get; set; } = string.Empty;

    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }

    [Required]
    [ForeignKey("Employes")]
    public int IdEmploye { get; set; }

    public virtual Employes Employe { get; set; } = null!;
    public string Status { get; internal set; } = "En attente";
    public string? AdminComment { get; internal set; }
}
