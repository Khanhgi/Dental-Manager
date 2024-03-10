using Dental_Manager.Models;
using Dental_Manager.Services;
using DoAnT4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> AddEmployeesAsync(int employeeId)
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

        [HttpPut("update/{employeeId}")]
        public async Task<IActionResult> UpdateEmployeeAsync(int employeeId, Employee updateModel)
        {
            var result = await _employeeServices.UpdateEmployeeAsync(employeeId, updateModel);

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

        [HttpDelete("delete/{employeeId}")]
        public async Task<IActionResult> DeleteEmployee(int employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);

            if (employee == null)
            {
                return NotFound();
            }

            employee.IsDeleted = true;
            employee.Status = false;
            _context.Entry(employeeId).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var deleteSuccessResponse = new
            {
                Message = "Vô hiệu hoá nhân viên thành công"
            };

            return Ok(deleteSuccessResponse);
        }

        [HttpDelete("deleteAll")]
        public async Task<IActionResult> DeleteEmployeeAsync([FromBody] List<int> employeeIds)
        {
            try
            {
                foreach (var employeeId in employeeIds)
                {
                    var result = await _employeeServices.DeleteEmployee(employeeId);
                }

                var deleteSuccessResponse = new
                {
                    Message = "Xoá nhân viên thành công"
                };

                return new OkObjectResult(deleteSuccessResponse);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error deleting staff: {ex.Message}");
                return new StatusCodeResult(500);
            }
        }


        [HttpGet("search")]
        public async Task<IActionResult> SearchEmployee(string keyword)
        {
            var result = await _employeeServices.SearchEmployee(keyword);

            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest("Invalid search result.");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterEmployeeAsync(Employee registerModel)
        {
            var result = await _loginEmployeeServices.RegisterEmployee(registerModel);

            if (result == null)
            {
                return StatusCode(500, "Internal Server Error");
            }

            if (result is OkObjectResult okResult)
            {
                return Ok(okResult.Value);
            }
            else if (result is BadRequestObjectResult badRequestResult)
            {
                return BadRequest(badRequestResult.Value);
            }

            return StatusCode(500, "Internal Server Error");
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginEmployeeAsync(Employee loginModel)
        {
            var result = await _loginEmployeeServices.LoginEmployee(loginModel);

            if (result is OkObjectResult okResult)
            {
                return Ok(okResult.Value);
            }
            else if (result is BadRequestObjectResult badRequestResult)
            {
                return BadRequest(badRequestResult.Value);
            }

            return StatusCode(500, "Internal Server Error");
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
