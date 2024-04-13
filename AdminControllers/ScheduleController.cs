using Dental_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace Dental_Manager.AdminControllers
{
    public class ScheduleController : Controller
    {
        QlkrContext qlkr = new QlkrContext();
        private readonly HttpClient _httpClient;

        public ScheduleController()
        {
            _httpClient = new HttpClient();
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                HttpContext.Session.SetString("ReturnUrl", Url.Action("Create", "Schedule"));

                return RedirectToAction("Login", "Employee");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeSchedule registrationModel)
        {
            var apiUrl = "https://localhost:7044/api/ScheduleApi/create";


            var existingTime = await qlkr.EmployeeSchedules.AnyAsync(s => s.Time == registrationModel.Time);

            if (registrationModel.Time == null ||
                registrationModel.Time.Value < TimeSpan.Zero ||
                registrationModel.Time.Value >= TimeSpan.FromDays(1))
            {
                ModelState.AddModelError("Time", "Time must be between 00:00:00.0000000 and 23:59:59.9999999.");
            }
            else if (existingTime)
            {
                ModelState.AddModelError("Time", "Time already exists in the database.");
            }

            if (!ModelState.IsValid)
            {
                return View(registrationModel);
            }


            var json = JsonConvert.SerializeObject(registrationModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("API Response Content: " + responseContent);


                var errorResponse = JsonConvert.DeserializeObject<object>(responseContent);

                ModelState.AddModelError("", errorResponse.ToString());
                return View(registrationModel);
            }
        }

        public async Task<IActionResult> Delete(int scheduleId)
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                HttpContext.Session.SetString("ReturnUrl", Url.Action("Delete", "Schedule", new { scheduleId }));

                return RedirectToAction("Login", "Staff");
            }
            var apiUrl = $"https://localhost:7044/api/ScheduleApi/delete/{scheduleId}";

            var response = await _httpClient.DeleteAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
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

        public IActionResult Index()
        {
            return View();
        }
    }
}
