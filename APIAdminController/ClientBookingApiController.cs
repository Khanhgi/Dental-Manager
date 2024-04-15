using Dental_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dental_Manager.APIAdminController
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientBookingApiController : Controller
    {
        private readonly QlkrContext _context;

        public ClientBookingApiController(QlkrContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBookingFromClient()
        {
            var bookings = await _context.Appointments
                 .Include(b => b.Clinic)
                 .Include(b => b.Patient)
                 .Include(b => b.Employee)
                 .ToListAsync();

            var bookingFromClientsWithFullInfo = bookings.Select(s => new
            {
                s.AppointmentId,
                s.PatientId,
                s.EmployeeId,
                s.Name,
                s.Phone,
                s.AppointmentDate,
                s.Status,
                s.AppointmentCreatedDate,
                s.ClinicId,

                Clinic = s.Clinic != null ? new
                {
                    s.Clinic.ClinicName,
                    s.Clinic.ClinicAddress,
                    s.Clinic.ClinicPhone
                } : null,

                Patient = s.Patient != null ? new
                {
                    s.Patient.PatientName,
                    s.Patient.PatientId
                } : null,

                Employee = s.Employee != null ? new
                {
                    s.Employee.EmployeeName,
                    s.Employee.EmployeeId
                } : null

            }).ToList();

            return Ok(bookingFromClientsWithFullInfo);
        }

        [HttpPut("update/{bookingId}")]
        public async Task<IActionResult> UpdateBookingFromClient(int bookingId)
        {
            var booking = await _context.Appointments.FindAsync(bookingId);

            if (booking == null)
            {
                return NotFound();
            }
            booking.Status = false;
            _context.Entry(booking).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var updateSuccessResponse = new
            {
                Message = "Booking status updated successfully"
            };

            return Ok(updateSuccessResponse);
        }
    }
}
