using Dental_Manager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dental_Manager.Services
{
    public class EmployeeServices
    {
        private readonly QlkrContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmployeeServices(QlkrContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        //Cập nhật nhân viên
        public async Task<IActionResult> UpdateEmployeeAsync(int EmployeeId, Employee updateModel)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == EmployeeId);

            if (employee == null)
            {
                return new NotFoundResult();
            }

            employee.EmployeeName = updateModel.EmployeeName;
            employee.EmployeeEmail = updateModel.EmployeeEmail;
            employee.EmployeePhone = updateModel.EmployeePhone;
            if (updateModel.RoleId != employee.RoleId)
            {
                var newRole = await _context.Roles.FindAsync(updateModel.RoleId);
                if (newRole != null)
                {
                    employee.Role = newRole;
                }
            }
            if (updateModel.ClinicId != employee.ClinicId)
            {
                var newClinic = await _context.Clinics.FindAsync(updateModel.ClinicId);
                if (newClinic != null)
                {
                    employee.Clinic = newClinic;
                }
            }

            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var updateSuccessResponse = new
            {
                Message = "Employee updated successfully"
            };

            return new OkObjectResult(updateSuccessResponse);
        }


        //Tìm kiếm nhân viên
        public async Task<List<object>> SearchEmployee(string keyword)
        {
            var employees = await _context.Employees
                .Include(p => p.Clinic)
                .Include(p => p.Role)
                .Where(p => p.EmployeeName.Contains(keyword) || p.EmployeeId.ToString() == keyword)
                .ToListAsync();

            var employeeInfo = employees.Select(p => (object)new
            {
                p.EmployeeId,
                p.EmployeeName,
                p.EmployeeEmail,
                p.EmployeePhone,
                Role = new
                {
                    Role = p.Role.RoleId,
                    Name = p.Role.Name
                },
                Clinic = new
                {
                    Name = p.Clinic.ClinicName,
                    Address = p.Clinic.ClinicAddress
                }
            }).ToList();

            return employeeInfo;
        }

        public async Task<List<object>> GetAllEmployee()
        {
            var employee = await _context.Employees.Include(l => l.Clinic).Include(l => l.Role).ToListAsync();

            return employee.Select(l => new
            {
                l.EmployeeId,
                l.EmployeeName,
                l.EmployeeEmail,
                l.EmployeePhone,
                l.EmployeeAddress,
                Role = new
                {
                    Role = l.Role.RoleId,
                    Name = l.Role.Name
                },
                Clinic = new
                {
                    ClinicID = l.Clinic.ClinicId,
                    Name = l.Clinic.ClinicName,
                    Address = l.Clinic.ClinicAddress
                }
            }).Cast<object>().ToList();
        }

        [HttpDelete("delete/{employeeId}")]
        public async Task<IActionResult> DeleteEmployee(int employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);

            if (employee == null)
            {
                return new NotFoundResult();
            }

            employee.IsDeleted = true;
            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var deleteSuccessResponse = new
            {
                Message = "Employee marked as deleted successfully"
            };

            return new OkObjectResult(deleteSuccessResponse);
        }

        [HttpPut("add/{employeeId}")]
        public async Task<IActionResult> AddEmployee(int employeeId)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

            if (employee == null)
            {
                return new NotFoundResult();
            }

            employee.IsDeleted = false;
            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var addSuccessResponse = new
            {
                Message = "Employee added successfully"
            };

            return new OkObjectResult(addSuccessResponse);
        }

    }
}
