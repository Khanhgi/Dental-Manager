using Dental_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit.Cryptography;

namespace Dental_Manager.Services
{
    public class ServiceTypeServices
    {
        private readonly QlkrContext _qlkrContext;

        public ServiceTypeServices(QlkrContext qlkrContext)
        {
            _qlkrContext = qlkrContext;
        }

        public async Task<List<object>> GetAllServicesType()
        {
            var servicetypes = await _qlkrContext.ServiceTypes.ToListAsync();
            return servicetypes.Select(s => new
            {
                s.ServiceTypeId,
                s.Name,
            }).Cast<object>().ToList();
        }

        public async Task<IActionResult> CreateServicesType(ServiceType servicetype)
        {
            try
            {
                _qlkrContext.ServiceTypes.Add(servicetype);
                await _qlkrContext.SaveChangesAsync();

                var CreatedServicesType = await _qlkrContext.ServiceTypes
                    .FirstOrDefaultAsync(p => p.ServiceTypeId == servicetype.ServiceTypeId);

                if (CreatedServicesType != null)
                {
                    var result = new
                    {
                        CreatedServicesType.ServiceTypeId,
                        CreatedServicesType.Name,
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
                Console.Error.WriteLine($"Error creating servicetype: {ex.Message}");
                return new StatusCodeResult(500);
            }
        }

        public async Task<IActionResult> UpdateServicesType(int servicesTypeId, ServiceType servicetype)
        {

            var ServicesTypeUpdate = await _qlkrContext.ServiceTypes
                .FirstOrDefaultAsync(x => x.ServiceTypeId == servicesTypeId);

            if (ServicesTypeUpdate == null)
            {
                return new NotFoundObjectResult("Not found ServicesType");
            }

            if (!string.IsNullOrWhiteSpace(servicetype.Name))
            {
                ServicesTypeUpdate.Name = servicetype.Name;
            }

            _qlkrContext.Entry(ServicesTypeUpdate).State = EntityState.Modified;
            await _qlkrContext.SaveChangesAsync();

            var updateSuccessResponse = new
            {
                Message = "Servcies type updated successfully",
                Name = servicetype.Name
            };

            return new OkObjectResult(updateSuccessResponse);
        }
    }
}
