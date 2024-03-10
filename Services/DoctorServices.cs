using Dental_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace Dental_Manager.Services
{
    public class DoctorServices
    {
        private readonly QlkrContext _context;
        private readonly IHttpContextAccessor _httpcontextAccessor;

        public DoctorServices(QlkrContext context, IHttpContextAccessor httpcontextAccessor)
        {
            _context = context;
            _httpcontextAccessor = httpcontextAccessor;
        }

        public async Task<IActionResult> UpdateDoctorAsync(int DoctorId, Doctor updateModel)
        {
            var doctor = await _context.Doctors.FirstOrDefaultAsync(e => e.DoctorId == DoctorId);

            if (doctor == null)
            {
                return new NotFoundResult();
            }

            doctor.DoctorName = updateModel.DoctorName;
            doctor.DoctorSpecialty = updateModel.DoctorSpecialty;

            if (updateModel.RoleId != doctor.RoleId)
            {
                var newRole = await _context.Roles.FindAsync(updateModel.RoleId);
                if (newRole != null)
                {
                    doctor.Role = newRole;
                }
            }
            if (updateModel.ClinicId != doctor.ClinicId)
            {
                var newClinic = await _context.Clinics.FindAsync(updateModel.ClinicId);
                if (newClinic != null)
                {
                    doctor.Clinic = newClinic;
                }
            }

            _context.Entry(doctor).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var updateSuccessResponse = new
            {
                Message = "Doctor updated successfully"
            };

            return new OkObjectResult(updateSuccessResponse);
        }


        //Tìm kiếm bác sĩ
        public async Task<List<object>> searchDoctor(string keyword)
        {
            var doctors = await _context.Doctors
                .Include(p => p.Clinic)
                .Include(p => p.Role)
                .Where(p => p.DoctorName.Contains(keyword))
                .ToListAsync();

            var doctorInfo = doctors.Select(p => (object)new
            {
                p.DoctorId,
                p.DoctorName,
                p.DoctorSpecialty,

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

            return doctorInfo;
        }

        public async Task<List<object>> GetAllDoctor()
        {
            var doctor = await _context.Doctors.Include(l => l.Clinic).Include(l => l.Role).ToListAsync();

            return doctor.Select(l => new
            {
                l.DoctorId,
                l.DoctorName,
                l.DoctorSpecialty,

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

        [HttpDelete("delete/{doctorId}")]
        public async Task<IActionResult> DeleteDoctor(int doctorId)
        {
            var doctor = await _context.Doctors.FindAsync(doctorId);

            if (doctor == null)
            {
                return new NotFoundResult();
            }

            var doctorName = doctor.DoctorName;

            doctor.IsDeleted = true;
            _context.Entry(doctor).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var deleteSuccessResponse = new
            {
                Message = $"Doctor {doctorName} marked as deleted successfully"
            };

            return new OkObjectResult(deleteSuccessResponse);
        }

        [HttpPut("add/{doctorId}")]
        public async Task<IActionResult> AddEmployee(int doctorId)
        {
            var doctor = await _context.Doctors.FirstOrDefaultAsync(e => e.DoctorId == doctorId);

            if (doctor == null)
            {
                return new NotFoundResult();
            }

            var doctorName = doctor.DoctorName; 

            doctor.IsDeleted = false;

            _context.Entry(doctor).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var addSuccessResponse = new
            {
                Message = $"Doctor {doctorName} added successfully"
            };

            return new OkObjectResult(addSuccessResponse);
        }
    }
}
