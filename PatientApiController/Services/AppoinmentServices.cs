using Dental_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dental_Manager.PatientApiController.Mail;

namespace Dental_Manager.PatientApiController.Services
{
    public class AppoinmentServices
    {
        private readonly QlkrContext _qlkrContext;
        private readonly SendMail _sendMail;

        public AppoinmentServices(QlkrContext qlkrContext, SendMail sendMail)
        {
            _qlkrContext = qlkrContext;
            _sendMail = sendMail;
        }

        public async Task<IActionResult> CreateBooking([FromBody] Appointment registrationModel)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(registrationModel.Phone) || string.IsNullOrWhiteSpace(registrationModel.Name))
                {
                    var errorResponse = new
                    {
                        Message = "Info cannot be empty"
                    };

                    return new BadRequestObjectResult(errorResponse);
                }



                var patient = await _qlkrContext.Patients.FindAsync(registrationModel.PatientId);
                var employee = await _qlkrContext.Employees.FindAsync(registrationModel.EmployeeId);
                var clinic = await _qlkrContext.Clinics.FindAsync(registrationModel.ClinicId);

                if (patient == null || employee == null || clinic == null)
                {
                    var Check = new List<string>();
                    if (patient == null) Check.Add("Patient");
                    if (employee == null) Check.Add("Employee");
                    if (clinic == null) Check.Add("Clinic");

                    return new NotFoundObjectResult($"Thieu tp nay: {string.Join(", ", Check)}.");
                }

                var existingBooking = await _qlkrContext.Appointments
                    .FirstOrDefaultAsync(b => b.EmployeeId == registrationModel.EmployeeId &&
                                              b.AppointmentDate == registrationModel.AppointmentDate);

                if (existingBooking != null)
                {
                    if (registrationModel.IsBooking == false) // nếu isBoooking == false thì cho phép user book tiếp nhân viên đó

                    {
                        var newBooking = new Appointment
                        {
                            Patient = patient,
                            Employee = employee,
                            Clinic = clinic,
                            Name = registrationModel.Name,
                            Phone = registrationModel.Phone,
                            AppointmentDate = registrationModel.AppointmentDate,
                            Note = registrationModel.Note,
                            Status = registrationModel.Status,
                            AppointmentCreatedDate = DateTime.Now
                        };

                        _qlkrContext.Appointments.Add(newBooking);
                        await _qlkrContext.SaveChangesAsync();
                        _sendMail.SendAppoinmentNotificationEmail(employee.EmployeeEmail, registrationModel);


                        if (patient != null)
                        {
                            _sendMail.SendAppoinmentConfirmationEmail(patient.PatientEmail, registrationModel);
                        }
                        var registrationSuccessResponse = new
                        {
                            Message = "Registration successful",
                            BookingId = newBooking.AppointmentId,
                            ClientId = newBooking.PatientId,
                            
                            Employee = new
                            {
                                EmployeeId = newBooking.Employee?.EmployeeId,
                                Name = newBooking.Employee?.EmployeeName,
                                Phone = newBooking.Employee?.EmployeePhone,
                            },
                            
                            Clinic = new
                            {
                                BranchId = newBooking.Clinic?.ClinicId,
                                Address = newBooking.Clinic?.ClinicAddress,
                                Hotline = newBooking.Clinic?.ClinicPhone
                            },

                            Name = registrationModel.Name,
                            Phone = registrationModel.Phone,
                            DateTime = registrationModel.AppointmentDate,
                            Note = registrationModel.Note,
                            Status = registrationModel.Status,
                            CreatedAt = newBooking.AppointmentCreatedDate,
                        };

                        return new OkObjectResult(registrationSuccessResponse);
                    }
                    else
                    {
                        return new BadRequestObjectResult("Có nha sĩ đã được đặt vào thời gian này.");
                    }
                }

                else
                {
                    var scheduleDetails = await _qlkrContext.EmployeeScheduleDetails
                                .AnyAsync(sd => sd.EmployeeId == registrationModel.EmployeeId &&
                               sd.Date == registrationModel.AppointmentDate);

                    if (scheduleDetails)
                    {
                        var newBooking = new Appointment
                        {
                            Patient = patient,
                            Employee = employee,
                            Clinic = clinic,
                            Name = registrationModel.Name,
                            Phone = registrationModel.Phone,
                            AppointmentDate = registrationModel.AppointmentDate,
                            Note = registrationModel.Note,
                            Status = registrationModel.Status,
                            AppointmentCreatedDate = DateTime.Now
                        };

                        _qlkrContext.Appointments.Add(newBooking);
                        await _qlkrContext.SaveChangesAsync();
                        _sendMail.SendAppoinmentNotificationEmail(employee.EmployeeEmail, registrationModel);


                        if (patient != null)
                        {
                            _sendMail.SendAppoinmentConfirmationEmail(patient.PatientEmail, registrationModel);
                        }
                        var registrationSuccessResponse = new
                        {
                            Message = "Registration successful",
                            BookingId = newBooking.AppointmentId,
                            ClientId = newBooking.PatientId,

                            Employee = new
                            {
                                StaffId = newBooking.Employee?.EmployeeId,
                                Name = newBooking.Employee?.EmployeeName,
                                Phone = newBooking.Employee?.EmployeePhone,
                            },

                            Clinic = new
                            {
                                BranchId = newBooking.Clinic?.ClinicId,
                                Address = newBooking.Clinic?.ClinicAddress,
                                Hotline = newBooking.Clinic?.ClinicPhone
                            },

                            Name = registrationModel.Name,
                            Phone = registrationModel.Phone,
                            DateTime = registrationModel.AppointmentDate,
                            Note = registrationModel.Note,
                            Status = registrationModel.Status,
                            CreatedAt = newBooking.AppointmentCreatedDate,
                        };

                        return new OkObjectResult(registrationSuccessResponse);
                    }
                    else
                    {
                        return new BadRequestObjectResult("Không có nha sĩ nào có lịch làm việc vào thời gian này.");
                    }
                }

            }
            catch (Exception ex)
            {
                return new ObjectResult($"An error occurred: {ex.Message}") { StatusCode = 500 };
            }

        }

        public async Task<List<object>> GetAllAppointment()
        {
            var AllBookings = await _qlkrContext.Appointments
                .Include(s => s.Employee)
                .Include(s => s.Patient)
                .Include(s => s.Clinic)
                .ToListAsync();

            return AllBookings.Select(s => new
            {
                s.AppointmentId,
                s.Name,
                s.Phone,
                s.AppointmentDate,
                s.Note,
                s.Status,
                s.AppointmentCreatedDate,
                s.EmployeeId,
                s.PatientId,

                Employee = new
                {
                    s.Employee?.EmployeeId,
                    s.Employee?.EmployeeName
                },

                Patient = new
                {
                    s.Patient?.PatientId,
                    s.Patient?.PatientName
                },

                Clinic = new
                {
                    s.Clinic?.ClinicId,
                    s.Clinic?.ClinicAddress,
                    s.Clinic?.ClinicPhone
                }
            }).Cast<object>().ToList();
        }

        public async Task<List<object>> GetClinics()
        {
            var clinics = await _qlkrContext.Clinics.ToListAsync();

            return clinics.Select(s => new
            {
                ClinicId = s.ClinicId,
                Address = s.ClinicAddress,
                Hotline = s.ClinicPhone
            }).Cast<object>().ToList();
        }


        public async Task<IActionResult> UpdateBookingPatient(int appointmentId, Appointment updateModel)
        {
            var booking = await _qlkrContext.Appointments
               .FirstOrDefaultAsync(p => p.AppointmentId == appointmentId);

            if (booking == null)
            {
                return new NotFoundResult();
            }

            booking.Name = updateModel.Name;
            booking.Phone = updateModel.Phone;
            booking.Note = updateModel.Note;
            booking.Status = updateModel.Status;
            booking.AppointmentDate = DateTime.Now;

            if (updateModel.ClinicId != booking.ClinicId)
            {
                var newBranch = await _qlkrContext.Clinics.FindAsync(updateModel.ClinicId);
                if (newBranch != null)
                {
                    booking.Clinic = newBranch;
                }
            }

            if (updateModel.PatientId != booking.PatientId)
            {
                var newClient = await _qlkrContext.Patients.FindAsync(updateModel.PatientId);
                if (newClient != null)
                {
                    booking.Patient = newClient;
                }
            }

            if (updateModel.EmployeeId != booking.EmployeeId)
            {
                var newStaff = await _qlkrContext.Employees.FindAsync(updateModel.EmployeeId);
                if (newStaff != null)
                {
                    booking.Employee = newStaff;
                }
            }

            _qlkrContext.Entry(booking).State = EntityState.Modified;

            await _qlkrContext.SaveChangesAsync();

            var updateSuccessResponse = new
            {
                Message = "booking updated successfully"
            };

            return new OkObjectResult(updateSuccessResponse);
        }

    }
}
