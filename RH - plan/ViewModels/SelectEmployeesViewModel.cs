namespace HR_Management_System.ViewModels
{
    public class SelectEmployeesViewModel
    {
        public int ContraintesPlanningId { get; set; }
        public List<SelectEmployeeViewModel> Employees { get; set; }
    }

    public class SelectEmployeeViewModel
    {
        public int EmployeeId { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public bool IsSelected { get; set; }
    }
}
