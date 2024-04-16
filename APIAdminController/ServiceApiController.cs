using Dental_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dental_Manager.APIAdminController
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceApiController : Controller
    {
        private readonly QlkrContext _context;
        public ServiceApiController(QlkrContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllService()
        {
            var services = await _context.Services
                .Include(s => s.ServiceType)
                .ToListAsync();

            var servicesWithFullInfo = services.Select(s => new
            {
                s.ServiceId,
                s.ServiceName,
                s.ServicePrice,
                s.ServiceStatus,
                Servicetype = new
                {
                    s.ServiceType.ServiceTypeId,
                    s.ServiceType.Name
                }
            }).ToList();

            return Ok(servicesWithFullInfo);
        }

        [HttpPost("create")]
        public async Task<IActionResult> createService(Service registrationModel)
        {
            if (ModelState.IsValid)
            {

                var serviceType = await _context.ServiceTypes.FindAsync(registrationModel.ServiceTypeId);

                var newService = new Service
                {
                    ServiceName = registrationModel.ServiceName,
                    ServiceStatus = true,
                    ServicePrice = registrationModel.ServicePrice,
                    ServiceType = serviceType,
                };

                _context.Services.Add(newService);
                await _context.SaveChangesAsync();

                _context.Entry(newService).Reference(s => s.ServiceType).Load();

                var registrationSuccessResponse = new
                {
                    Message = "Registration successful",
                    ServiceId = newService.ServiceId,
                    Servicetype = new
                    {
                        Name = newService.ServiceType?.Name,
                    }

                };
                return Ok(registrationSuccessResponse);
            }

            var invalidDataErrorResponse = new
            {
                Message = "Invalid registration data",
                Errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            };
            return BadRequest(invalidDataErrorResponse);
        }

        [HttpDelete("delete/{serviceId}")]
        public async Task<IActionResult> Deleteservice(int serviceId)
        {
            var service = await _context.Services.FindAsync(serviceId);

            if (service == null)
            {
                return NotFound();
            }

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            var deleteSuccessResponse = new
            {
                Message = "Service deleted successfully"
            };

            return Ok(deleteSuccessResponse);
        }

        [HttpPut("update/{serviceId}")]
        public async Task<IActionResult> UpdateServices(int serviceId, Service updateModel)
        {
            var Services = await _context.Services
                .FirstOrDefaultAsync(p => p.ServiceId == serviceId);

            if (Services == null)
            {
                return NotFound();
            }

            Services.ServiceName = updateModel.ServiceName;
            Services.ServicePrice = updateModel.ServicePrice;
            if (updateModel.ServiceTypeId != Services.ServiceTypeId)
            {
                var newServices = await _context.ServiceTypes.FindAsync(updateModel.ServiceTypeId);
                if (newServices != null)
                {
                    Services.ServiceType = newServices;
                }
            }

            _context.Entry(Services).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var updateSuccessResponse = new
            {
                Message = "Services updated successfully"
            };

            return Ok(updateSuccessResponse);
        }
    }
}
