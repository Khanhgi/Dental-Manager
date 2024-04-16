using Dental_Manager.Models;
using Dental_Manager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dental_Manager.APIAdminController
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceTypeApiController : Controller
    {
        private readonly QlkrContext _qlkrContext;
        private readonly ServiceTypeServices _servicetypeservices;

        public ServiceTypeApiController(QlkrContext qlkrContext, ServiceTypeServices servicetypeservices)
        {
            _qlkrContext = qlkrContext;
            _servicetypeservices = servicetypeservices;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllServicesType()
        {
            var ServiceTypeInfo = await _servicetypeservices.GetAllServicesType();
            return Ok(ServiceTypeInfo);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateProductsType(ServiceType registrationModel)
        {

            var result = await _servicetypeservices.CreateServicesType(registrationModel);

            if (result is OkObjectResult okResult)
            {
                return Ok(okResult.Value);
            }
            else if (result is BadRequestObjectResult badRequestObjectResult)
            {
                return BadRequest(badRequestObjectResult.Value);
            }
            return StatusCode(500, "Internal Server Error");
        }

        [HttpPut("update/{serviceTypeId}")]
        public async Task<IActionResult> UpdateServicesTypesAsync(int serviceTypeId, ServiceType servicetype)
        {

            var result = await _servicetypeservices.UpdateServicesType(serviceTypeId, servicetype);

            if (result is OkObjectResult okResult)
            {

                return Ok(okResult.Value);

            }
            else if (result is NotFoundObjectResult notFoundResult)
            {

                return NotFound(notFoundResult.Value);

            }
            else
            {

                return StatusCode(500, "Internal Server Error");

            }
        }

        [HttpDelete("delete/{Servicetypeid}")]
        public async Task<IActionResult> DeleteServicesType(int Servicetypeid)
        {
            var Servicetype = await _qlkrContext.ServiceTypes.FindAsync(Servicetypeid);
            if (Servicetype == null)
            {
                return NotFound();
            }

            _qlkrContext.ServiceTypes.Remove(Servicetype);
            await _qlkrContext.SaveChangesAsync();

            var deleteSuccessResponse = new
            {
                Message = "Servicetype deleted successfully"
            };

            return Ok(deleteSuccessResponse);
        }
    }
}
