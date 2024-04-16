using Dental_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace Dental_Manager.AdminControllers
{
    public class ServiceTypeController : Controller
    {
        QlkrContext qlkr = new QlkrContext();
        private readonly HttpClient _httpClient;

        public ServiceTypeController()
        {
            _httpClient = new HttpClient();
        }

        public async Task<IActionResult> Index()
        {

            var apiResponse = await _httpClient.GetAsync($"https://localhost:7044/ServiceTypeApi");
            if (apiResponse.IsSuccessStatusCode)
            {
                var responseContent = await apiResponse.Content.ReadAsStringAsync();
                var ServicesType = JsonConvert.DeserializeObject<List<ServiceType>>(responseContent);

                return View(ServicesType);
            }
            else
            {
                var ServicesType = await qlkr.ServiceTypes
                   .ToListAsync();
                return View(ServicesType);
            }
        }

        public IActionResult Create()
        {
            var model = new ServiceType();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ServiceType registrationModel)
        {
            var apiUrl = $"https://localhost:7044/ServiceTypeApi/create";
            var serviceName = registrationModel.Name?.Trim();
            var checkSer = qlkr.ServiceTypes.FirstOrDefault(x => x.Name == serviceName);
            if (checkSer != null)
            {
                ModelState.AddModelError("Name", "Servicetypes with this name already exists.");
                return View(registrationModel);
            }
            if (string.IsNullOrEmpty(registrationModel.Name))
            {
                ModelState.AddModelError("Name", "Name cannot be empty.");
            }
            if (!ModelState.IsValid)
            {
                var model = new ServiceType();
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

                if (errorResponse != null)
                {
                    ModelState.AddModelError("", errorResponse.ToString());
                }

                return View(registrationModel);
            }
        }

        [HttpGet]
        public IActionResult Edit(int Servicetypeid)
        {

            var Servicetype = qlkr.ServiceTypes.Find(Servicetypeid);
            if (Servicetype == null)
            {
                return NotFound();
            }
            return View(Servicetype);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int Servicetypeid, ServiceType updateModel)
        {
            if (!ModelState.IsValid)
            {
                return View(updateModel);
            }

            var apiUrl = $"https://localhost:7044/ServiceTypeApi/update/{Servicetypeid}";

            var json = JsonConvert.SerializeObject(updateModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(apiUrl, content);

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
                return View(updateModel);
            }
        }

        public async Task<IActionResult> Delete(int Servicetypeid)
        {
            var apiUrl = $"https://localhost:7044/ServiceTypeApi/delete/{Servicetypeid}";

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
