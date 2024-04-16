using Dental_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dental_Manager.APIAdminController
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleApiController : Controller
    {
        private readonly QlkrContext _qlkrContext;

        public ScheduleApiController(QlkrContext qlkrContext)
        {
            _qlkrContext = qlkrContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSchedule()
        {
            var schedule = await _qlkrContext.EmployeeSchedules
                .Include(s => s.EmployeeScheduleDetails)
                .ToListAsync();

            var SchedulesWithFullInfo = schedule.Select(s => new
            {
                s.EmployeeScheduleId,
                s.Time,
            }).ToList();
            return Ok(SchedulesWithFullInfo);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSchedule([FromBody] EmployeeSchedule createModel)
        {
            if (ModelState.IsValid)
            {
                var ScheduleExists = await _qlkrContext.EmployeeSchedules.AnyAsync(b => b.Time == createModel.Time);
                if (ScheduleExists)
                {
                    return BadRequest(new { Message = "Schedules already exists." });
                }

                var newSchedule = new EmployeeSchedule
                {
                    EmployeeScheduleId = createModel.EmployeeScheduleId,
                    Time = createModel.Time,
                };

                _qlkrContext.EmployeeSchedules.Add(newSchedule);
                await _qlkrContext.SaveChangesAsync();

                var registrationSuccessResponse = new
                {
                    Message = "Schedules created successfully",
                    EmployeeScheduleId = newSchedule.EmployeeScheduleId
                };
                return Ok(registrationSuccessResponse);
            }

            var invalidDataErrorResponse = new
            {
                Message = "Invalid Schedule data",
                Errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            };
            return BadRequest(invalidDataErrorResponse);
        }


        [HttpPut("update/{EmployeeScheduleId}")]
        public async Task<IActionResult> UpdateSchedul(int EmployeeScheduleId, EmployeeSchedule updateModel)
        {
            var Schedule = await _qlkrContext.EmployeeSchedules.FindAsync(EmployeeScheduleId);
            if (Schedule == null)
            {
                return NotFound();
            }

            if (updateModel.Time.HasValue)
            {
                Schedule.Time = updateModel.Time;
            }
            _qlkrContext.Entry(Schedule).State = EntityState.Modified;
            await _qlkrContext.SaveChangesAsync();

            var updateSuccessResponse = new
            {
                Message = "Schedule updated successfully"
            };

            return Ok(updateSuccessResponse);
        }


        [HttpDelete("delete/{EmployeeScheduleId}")]
        public async Task<IActionResult> DeleteSchedule(int EmployeeScheduleId)
        {
            var Schedules = await _qlkrContext.EmployeeSchedules.FindAsync(EmployeeScheduleId);
            if (Schedules == null)
            {
                return NotFound();
            }

            _qlkrContext.EmployeeSchedules.Remove(Schedules);
            await _qlkrContext.SaveChangesAsync();

            var deleteSuccessResponse = new
            {
                Message = "Schedule deleted successfully"
            };

            return Ok(deleteSuccessResponse);
        }
       

    }
}
