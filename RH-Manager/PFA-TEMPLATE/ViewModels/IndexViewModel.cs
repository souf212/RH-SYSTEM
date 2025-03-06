using PFA_TEMPLATE.Models;

namespace PFA_TEMPLATE.ViewModels
{
    public class IndexViewModel
    {
        public List<Conges> LeaveRequests { get; set; }
        public int TotalEmployees { get; set; }
        public int ActiveTasks { get; set; }
        public int TotalLeaveRequests { get; set; }

    }
}