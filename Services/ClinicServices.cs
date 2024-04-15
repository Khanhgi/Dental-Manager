using Dental_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dental_Manager.Services
{
    public class ClinicServices
    {
        private readonly QlkrContext _context;

        public ClinicServices(QlkrContext context)
        {
            _context = context;
        }

        public async Task<List<object>> GetAllClinic()
        {
            var clinic = await _context.Clinics.ToListAsync();

            return clinic.Select(p => new
            {
                p.ClinicId,
                p.ClinicName,
                p.ClinicAddress,
                p.ClinicPhone,
            }).Cast<object>().ToList();
        }

        public async Task<object> CreateClinic(Clinic createModel)
        {
            try
            {
                _context.Clinics.Add(createModel);
                await _context.SaveChangesAsync();

                var createdClinic = await _context.Clinics.FirstOrDefaultAsync(p => p.ClinicId == createModel.ClinicId);

                if (createdClinic != null)
                {
                    var result = new
                    {
                        createdClinic.ClinicId,
                        createdClinic.ClinicName,
                        createdClinic.ClinicAddress,
                        createdClinic.ClinicPhone,
                    };
                    return new OkObjectResult(result);
                }
                else
                {
                    return new NotFoundResult();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error creating product: {ex.Message}");
                return new StatusCodeResult(500);
            }
        }

        public async Task<IActionResult> UpdateClinic(int ClinicId, Clinic clinic)
        {
            var ClinicUpdate = await _context.Clinics.FirstOrDefaultAsync(x => x.ClinicId == ClinicId);

            if (ClinicUpdate != null)
            {
                return new NotFoundObjectResult("Not found Clinic");
            }

            if (!string.IsNullOrWhiteSpace(clinic.ClinicName))
            {
                ClinicUpdate.ClinicName = clinic.ClinicName;
            }
            if (!string.IsNullOrWhiteSpace(clinic.ClinicAddress))
            {
                ClinicUpdate.ClinicAddress = clinic.ClinicAddress;
            }
            if (!string.IsNullOrWhiteSpace(clinic.ClinicPhone))
            {
                ClinicUpdate.ClinicPhone = clinic.ClinicPhone;
            }

            _context.Entry(ClinicUpdate).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var updateSuccessResponse = new
            {
                Message = "Clinic updated successfully",
                clinic.ClinicName,
                clinic.ClinicAddress,
                clinic.ClinicPhone,
            };

            return new OkObjectResult(updateSuccessResponse);
        }

        public async Task<IActionResult> DeleteAllClinicAsync(int clinicID)
        {
            var ClinicToDelete = await _context.Clinics.FindAsync(clinicID);

            if (ClinicToDelete == null)
            {
                return new NotFoundObjectResult("Not found Clinic");
            }

            _context.Clinics.Remove(ClinicToDelete);
            await _context.SaveChangesAsync();

            var DeleteSuccessResponse = new
            {
                Message = "Clinic deleted successfully"
            };

            return new OkObjectResult(DeleteSuccessResponse);
        }


        public async Task<IActionResult> DeleteClinicAsync(int clinicId)
        {
            try
            {
                var clinic = await _context.Clinics.FindAsync(clinicId);

                if (clinic == null)
                {
                    return new NotFoundObjectResult("Clinic not found.");
                }

                _context.Clinics.Remove(clinic);
                await _context.SaveChangesAsync();

                var DeleteSuccessResponse = new
                {
                    Message = "Clinic deleted successfully",
                };

                return new OkObjectResult(DeleteSuccessResponse);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error deleting Clinic: {ex.Message}");
                return new StatusCodeResult(500);
            }
        }
    }
}
