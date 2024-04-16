using Dental_Manager.PatientApiController.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dental_Manager.PatientApiController
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AppoinmentDateController : Controller
    {
        private readonly AppoinmentDateServices _appoinmentDateServices;

        public AppoinmentDateController(AppoinmentDateServices appoinmentDateServices)
        {
            _appoinmentDateServices = appoinmentDateServices;
        }

        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetScheduleEmployeeId(int employeeId)
        {
            var result = await _appoinmentDateServices.GetStaffAndSchedule(employeeId);
            return Ok(result);
        }
    }
}
