using Dental_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dental_Manager.PatientApiController.Services
{
    public class AppoinmentDateServices
    {
        private readonly QlkrContext _qlkrContext;

        public AppoinmentDateServices(QlkrContext qlkrContext)
        {
            _qlkrContext = qlkrContext;
        }

        public async Task<IActionResult> GetStaffAndSchedule(int employeeId)
        {
            try
            {
                // Lấy thông tin nhân viên
                var employee = await _qlkrContext.Employees
                    .Where(s => s.EmployeeId == employeeId)
                    .Select(s => new
                    {
                        s.EmployeeId,
                        s.EmployeeName,
                        s.EmployeePhone,
                        s.EmployeeEmail,
                        s.EmployeeAddress,
                        s.Avatar,
                        s.Status,
                        s.RoleId,
                        s.ClinicId
                    })
                    .FirstOrDefaultAsync();

                if (employee == null)
                {
                    return new NotFoundObjectResult($"Không tìm thấy thông tin cho nhân viên có ID: {employeeId}");
                }


                // Lấy lịch làm việc của nhân viên
                var schedule = await _qlkrContext.EmployeeScheduleDetails
                    .Include(sd => sd.EmployeeSchedule)
                    .Where(sd => sd.EmployeeId == employeeId)
                    .Select(sd => new
                    {
                        sd.EmployeeSchedule.EmployeeScheduleId,
                        sd.EmployeeSchedule.Time,
                        sd.Date,
                        sd.Status
                    })
                    .ToListAsync();

                var result = new
                {
                    Employee = employee,
                    EmployeeSchedule = schedule
                };

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return new ObjectResult($"Đã xảy ra lỗi: {ex.Message}") { StatusCode = 500 };
            }
        }
    }
}
