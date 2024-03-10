using Microsoft.AspNetCore.Mvc;

namespace Dental_Manager.AdminControllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
