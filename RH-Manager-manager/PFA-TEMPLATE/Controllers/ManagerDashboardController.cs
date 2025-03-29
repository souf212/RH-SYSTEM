using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PFA_TEMPLATE
{ 

    [Authorize(Roles = "Manager")]
    public class ManagerDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
