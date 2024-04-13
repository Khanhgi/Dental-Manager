using Microsoft.AspNetCore.Mvc;

namespace Dental_Manager.APIAdminController
{
    public class ScheduleApiController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
