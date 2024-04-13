using Dental_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MailKit.Net.Smtp;

namespace Dental_Manager.APIAdminController
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleDetailApiController : Controller
    {
        private readonly QlkrContext qlkr;

        public ScheduleDetailApiController(QlkrContext qlkrContext)
        {
            qlkr = qlkrContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllScheduleDetails()
        {
            var Schedules = await qlkr.EmployeeScheduleDetails
                 .Include(s => s.Employee)
                 .Include(s => s.EmployeeSchedule)
                .ToListAsync();

            var SchedulesWithFullInfo = Schedules.Select(s => new
            {
                s.EmployeeScheduleId,
                s.Date,
                s.EmployeeId,
                Employee = new
                {
                    s.Employee.EmployeeId,
                    s.Employee.EmployeeName
                },
                EmployeeSchedule = s.EmployeeSchedule != null ? new
                {
                    s.EmployeeSchedule.EmployeeScheduleId,
                    s.EmployeeSchedule.Time
                } : null
            }).ToList();
            return Ok(SchedulesWithFullInfo);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateScheduleDetail([FromBody] EmployeeScheduleDetail inputModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        Message = "Invalid Schedule Detail data",
                        Errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList()
                    });
                }

                var employee = await qlkr.Employees.FindAsync(inputModel.EmployeeId);
                var schedule = await qlkr.EmployeeSchedules.FindAsync(inputModel.EmployeeScheduleId);

                var existingDetail = await qlkr.EmployeeScheduleDetails
                    .FirstOrDefaultAsync(sd =>
                        sd.EmployeeId == inputModel.EmployeeId &&
                        sd.EmployeeScheduleId == inputModel.EmployeeScheduleId &&
                        sd.Date == inputModel.Date);

                if (existingDetail != null)
                {
                    return BadRequest(new
                    {
                        Message = "A schedule detail already exists for the selected Staff, Schedule, and Date."
                    });
                }

                var createModel = new EmployeeScheduleDetail
                {
                    EmployeeId = inputModel.EmployeeId,
                    EmployeeScheduleId = inputModel.EmployeeScheduleId,
                    Date = inputModel.Date,
                    Status = true,
                    Employee = employee,
                    EmployeeSchedule = schedule
                };

                qlkr.EmployeeScheduleDetails.Add(createModel);
                await qlkr.SaveChangesAsync();

                await SendEmailCreateAsync(employee.EmployeeName, employee.EmployeeEmail, createModel);


                var registrationSuccessResponse = new
                {
                    Message = "Schedule Detail registration successful",
                    ScheduleDetailId = createModel.EmployeeScheduleId,
                    Employee = new
                    {
                        EmployeeId = createModel.Employee?.EmployeeId,
                    },
                    EmployeeSchedule = new
                    {
                        Time = createModel.EmployeeSchedule?.Time,
                    }
                };

                return Ok(registrationSuccessResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateScheduleDetail([FromQuery] int scheduleId, [FromQuery] int employeeId, EmployeeScheduleDetail updatedModel)
        {
            try
            {
                var existingScheduleDetail = await qlkr.EmployeeScheduleDetails
                    .FirstOrDefaultAsync(s => s.EmployeeScheduleId == scheduleId && s.EmployeeId == employeeId);

                if (existingScheduleDetail == null)
                {
                    return NotFound(new
                    {
                        Message = "Schedule Detail not found."
                    });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        Message = "Invalid Schedule Detail data",
                        Errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList()
                    });
                }
                var employee = await qlkr.Employees.FindAsync(updatedModel.EmployeeId);
                var schedule = await qlkr.EmployeeSchedules.FindAsync(updatedModel.EmployeeScheduleId);

                existingScheduleDetail.Date = updatedModel.Date;
                existingScheduleDetail.Status = updatedModel.Status;

                await qlkr.SaveChangesAsync();

                await SendEmailUpdateAsync(employee.EmployeeName, employee.EmployeeEmail, existingScheduleDetail);

                var scheduleDetailIds = await qlkr.EmployeeScheduleDetails
                    .Select(sd => new { sd.EmployeeScheduleId, sd.EmployeeId })
                    .ToListAsync();

                return Ok(scheduleDetailIds);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteScheduleDetail([FromQuery] int employeeId, [FromQuery] int scheduleId)
        {
            try
            {
                var matchingScheduledetails = await qlkr.EmployeeScheduleDetails
                    .Where(s => s.EmployeeId == employeeId && s.EmployeeScheduleId == scheduleId)
                    .ToListAsync();

                if (matchingScheduledetails == null || !matchingScheduledetails.Any())
                {
                    return NotFound(new
                    {
                        Message = "Scheduledetail not found."
                    });
                }

                qlkr.EmployeeScheduleDetails.RemoveRange(matchingScheduledetails);
                await qlkr.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Scheduledetail deleted successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        private async Task SendEmailCreateAsync(string recipientName, string recipientEmail, EmployeeScheduleDetail scheduleDetail)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("Admin", "voankhanh0@gmail.com"));
            message.Subject = "Upcoming Work Schedule Notification";

            message.Body = new TextPart("html")
            {
                Text = $"<html><body>" +
                       $"<p><strong>Lịch làm việc của bạn: </strong></p>" +
                       $"<p><strong>Employee Name:</strong> {recipientName}</p>" +
                       $"<p><strong>Schedule Time:</strong> {scheduleDetail.EmployeeSchedule?.Time}</p>" +
                       $"<p><strong>Date:</strong> {scheduleDetail.Date?.ToString("dd/MM/yyyy")}</p>" +
                       $"</body></html>"
            };

            try
            {
                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, false);
                    client.Authenticate("voankhanh0@gmail.com", "seef sxno bsef ufyq");

                    message.To.Add(new MailboxAddress(recipientName, recipientEmail));

                    await client.SendAsync(message);

                    client.Disconnect(true);
                }
            }
            catch (Exception ex) { }

        }

        private async Task SendEmailUpdateAsync(string recipientName, string recipientEmail, EmployeeScheduleDetail scheduleDetail)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("Admin", "voankhanh0@gmail.com"));
            message.Subject = "Upcoming Work Schedule Notification";

            message.Body = new TextPart("html")
            {
                Text = $"<html><body>" +
                       $"<p><strong>Lịch làm việc của bạn đã được chỉnh sửa ! </strong></p>" +
                       $"<p><strong>Employee Name:</strong> {recipientName}</p>" +
                       $"<p><strong>Schedule Time:</strong> {scheduleDetail.EmployeeSchedule?.Time}</p>" +
                       $"<p><strong>Date:</strong> {scheduleDetail.Date?.ToString("dd/MM/yyyy")}</p>" +
                       $"</body></html>"
            };

            try
            {
                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, false);
                    client.Authenticate("voankhanh0@gmail.com", "seef sxno bsef ufyq");

                    message.To.Add(new MailboxAddress(recipientName, recipientEmail));

                    await client.SendAsync(message);

                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
            }
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}
