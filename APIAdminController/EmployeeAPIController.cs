using Dental_Manager.Models;
using Dental_Manager.Services;
using DoAnT4.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoAnT4n.APIAdminController
{
    [ApiController]
    [Route("api/[controller")]
    public class EmployeeAPIController : Controller
    {
        private readonly QlkrContext _context;
        private readonly LoginEmployeeServices _loginEmployeeServices;
        private readonly EmployeeServices _employeeServices;

        public EmployeeAPIController(QlkrContext context, LoginEmployeeServices loginEmployeeServices, EmployeeServices employeeServices)
        {
            _context = context;
            _loginEmployeeServices = loginEmployeeServices;
            _employeeServices = employeeServices;
        }


        [HttpPut("add/{employeeId}")]
        public async Task<IActionResult> addEmployeesAsync(int employeeId)
        {
            var result = await _employeeServices.AddEmployee(employeeId);

            if (result is OkObjectResult okResult)
            {
                return Ok(okResult.Value);
            }
            else if (result is NotFoundObjectResult notFoundResult)
            {
                return NotFound(notFoundResult.Value);
            }
            else
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var result = await _loginEmployeeServices.GetEmployeeInfoById(id);
            return result;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployee()
        {
            var employee = await _employeeServices.GetAllEmployee();
            return Ok(employee);
        }
        
    }
}
