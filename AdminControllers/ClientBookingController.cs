using Dental_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace Dental_Manager.AdminControllers
{
    public class ClientBookingController : Controller
    {
        QlkrContext qlkr = new QlkrContext();
        private readonly HttpClient _httpClient;

        public ClientBookingController()
        {
            _httpClient = new HttpClient();
        }

        public async Task<IActionResult> Index()
        {
            var apiResponse = await _httpClient.GetAsync("https://localhost:7044/api/ClientBookingApi");
            if (apiResponse.IsSuccessStatusCode)
            {
                var responseContent = await apiResponse.Content.ReadAsStringAsync();
                var bookingFromClients = JsonConvert.DeserializeObject<List<Appointment>>(responseContent);

                return View(bookingFromClients);
            }
            return View();
        }

        public async Task<IActionResult> Update(int bookingId)
        {
            var booking = await qlkr.Appointments.FirstOrDefaultAsync(s => s.AppointmentId == bookingId);
            
            if (booking == null)
            {
                return NotFound();
            }

            var apiUrl = $"https://localhost:7044/api/ClientBookingApi/update/{bookingId}";

            var response = await _httpClient.PutAsync(apiUrl, null); 

            if (response.IsSuccessStatusCode)
            {
                booking.Status = false;
                qlkr.Entry(booking).State = EntityState.Modified;
                await qlkr.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("API Response Content: " + responseContent);

                var errorResponse = JsonConvert.DeserializeObject<object>(responseContent);

                ModelState.AddModelError("", errorResponse.ToString());
                return RedirectToAction("Index");
            }
        }
    }
}
