using Dental_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace Dental_Manager.AdminControllers
{
    public class ServiceController : Controller
    {
        QlkrContext qlkr = new QlkrContext();
        private readonly HttpClient _httpClient;

        public ServiceController()
        {
            _httpClient = new HttpClient();
        }

        public async Task<IActionResult> Index()
        {
            var apiResponse = await _httpClient.GetAsync($"https://localhost:7044/ServiceApi");
            if (apiResponse.IsSuccessStatusCode)
            {
                var responseContent = await apiResponse.Content.ReadAsStringAsync();
                var services = JsonConvert.DeserializeObject<List<Service>>(responseContent);
                return View(services);
            }
            else
            {
                var servicesList = await qlkr.Services
                    .Include(s => s.ServiceType)
                    .ToListAsync();
                return View(servicesList);
            }
        }

        public IActionResult Create()
        {
            var serviceTypes = qlkr.ServiceTypes.ToList();
            ViewBag.serviceTypes = new SelectList(serviceTypes, "ServiceTypeId", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Service registrationModel)
        {
            var apiUrl = $"https://localhost:7044/ServiceApi/create";

            if (string.IsNullOrEmpty(registrationModel.ServiceName) && string.IsNullOrEmpty(registrationModel.ServicePrice.ToString()))
            {
                ModelState.AddModelError("ServiceName", "cannot be empty.");
                ModelState.AddModelError("ServicePrice", "cannot be empty.");
            }
            if (registrationModel.ServicePrice <= 0)
            {
                ModelState.AddModelError("ServicePrice", "Price must be greater than 0.");
            }

            if (string.IsNullOrEmpty(Request.Form["ServiceTypeId"]))
            {
                ModelState.AddModelError("ServiceTypeId", "ServiceType is required.");
            }
            if (ModelState.IsValid)
            {
                registrationModel.ServiceStatus = Request.Form["ServiceStatus"] == "true";
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

                    dynamic errorResponse = JsonConvert.DeserializeObject(responseContent);

                    if (errorResponse != null)
                    {
                        if (!string.IsNullOrEmpty(errorResponse.Message))
                        {
                            ModelState.AddModelError("", errorResponse.Message);
                        }
                        if (errorResponse.Errors != null)
                        {
                            foreach (var error in errorResponse.Errors)
                            {
                                ModelState.AddModelError("", error.ToString());
                            }
                        }
                    }

                    var serviceTypes = qlkr.ServiceTypes.ToList();
                    ViewBag.serviceTypes = new SelectList(serviceTypes, "ServiceTypeId", "Name");

                    return View(registrationModel);
                }
            }
            else
            {
                var serviceTypes = qlkr.ServiceTypes.ToList();
                ViewBag.serviceTypes = new SelectList(serviceTypes, "ServiceTypeId", "Name");
                return View(registrationModel);
            }
        }

        [HttpGet]
        public IActionResult Edit(int serviceId)
        {
            var service = qlkr.Services
              .Include(s => s.ServiceType)
              .FirstOrDefault(s => s.ServiceId == serviceId);

            if (service == null)
            {
                return NotFound();
            }

            var serviceTypes = qlkr.ServiceTypes.ToList();
            ViewBag.serviceTypes = new SelectList(serviceTypes, "ServiceTypeId", "Name");

            return View(service);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int serviceId, Service updateModel)
        {
            if (!ModelState.IsValid)
            {
                return View(updateModel);
            }
            updateModel.ServiceStatus = Request.Form["ServiceStatus"] == "true";

            var apiUrl = $"https://localhost:7044/ServiceApi/update/{serviceId}";

            var json = JsonConvert.SerializeObject(updateModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var updatedService = await qlkr.Services.FirstOrDefaultAsync(s => s.ServiceId == serviceId);
                    if (updatedService != null)
                    {
                        qlkr.Entry(updatedService).State = EntityState.Modified;
                        await qlkr.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving the editor's name: " + ex.Message);
                }
                return RedirectToAction("Index");
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("API Response Content: " + responseContent);

                var errorResponse = JsonConvert.DeserializeObject<object>(responseContent);

                ModelState.AddModelError("", errorResponse.ToString());
                return View(updateModel);
            }
        }

        public async Task<IActionResult> Delete(int serviceId)
        {

            var apiUrl = $"https://localhost:7044/ServiceApi/delete/{serviceId}";

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
    }
}
