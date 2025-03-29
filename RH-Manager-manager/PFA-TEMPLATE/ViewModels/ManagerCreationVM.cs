using System.ComponentModel.DataAnnotations;

namespace PFA_TEMPLATE.ViewModels
{
    public class ManagerCreationVM : UserVM
    {
        // List of employee IDs to be added to the manager's team
        [Display(Name = "Team Members")]
        public List<int> TeamMemberIds { get; set; } = new List<int>();
        public List<UserVM> AvailableEmployees { get; set; } = new List<UserVM>();

    }
}