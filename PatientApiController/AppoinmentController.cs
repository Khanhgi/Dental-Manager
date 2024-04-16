using Dental_Manager.Models;
using Dental_Manager.PatientApiController.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dental_Manager.PatientApiController
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AppoinmentController : Controller
    {
        private readonly QlkrContext _context;
        private readonly AppoinmentServices _appoinmentServices;

        public AppoinmentController(QlkrContext context, AppoinmentServices appoinmentServices)
        {
            _context =  context;
            _appoinmentServices = appoinmentServices;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAppointment()
        {
            var AllBookingFullInfo = await _appoinmentServices.GetAllAppointment();

            return Ok(AllBookingFullInfo);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] Appointment registrationModel)
        {
            var result = await _appoinmentServices.CreateBooking(registrationModel);

            if (result is OkObjectResult okResult)
            {
                return Ok(okResult.Value);
            }
            else if (result is BadRequestObjectResult badRequestResult)
            {
                return BadRequest(badRequestResult.Value);
            }

            return StatusCode(500, "Internal Server Error");
        }


        [HttpPut("update/{bookingId}")]
        public async Task<IActionResult> UpdateBookingClient(int bookingId, Appointment updateModel)
        {
            var result = await _appoinmentServices.UpdateBookingPatient(bookingId, updateModel);

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

        [HttpGet("clinics")]
        public async Task<IActionResult> GetClinics()
        {
            var BranchesFullInfo = await _appoinmentServices.GetClinics();

            return Ok(BranchesFullInfo);
        }
    }
}
