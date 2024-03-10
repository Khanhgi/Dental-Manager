using Dental_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace Dental_Manager.AdminControllers
{
    public class EmployeeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly QlkrContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
       
        public EmployeeController(QlkrContext context, IHttpContextAccessor contextAccessor)
        {
            _httpClient = new HttpClient();
            _context = context;
            _contextAccessor = contextAccessor;
        }

        public IActionResult Register()
        {
            var roles = _context.Roles.ToList();
            var clinics = _context.Clinics.ToList();
            ViewBag.Roles = new SelectList(roles, "RoleId", "Name");
            ViewBag.Clinics = new SelectList(clinics, "ClinicId", "ClinicAddress");
            string createdBy = HttpContext.Session.GetString("Name");
            ViewBag.CreatedBy = createdBy;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Employee registerModel)
        {
            var apiUrl = "https://localhost:7044/api/EmployeeAPI/register";

            registerModel.CreatedBy = HttpContext.Session.GetString("Name");

            var content = new StringContent(JsonConvert.SerializeObject(registerModel), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                registerModel.Status = Request.Form["Status"] == "true";
                var roles = _context.Roles.ToList();
                var clinics = _context.Clinics.ToList();
                ViewBag.Roles = new SelectList(roles, "RoleId", "Name");
                ViewBag.Clinics = new SelectList(clinics, "ClinicId", "ClinicAddress");
                var result = await response.Content.ReadAsStringAsync();
                return RedirectToAction("Index");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                dynamic errorResponse = JsonConvert.DeserializeObject(errorContent);
                string errorMessage = errorResponse.message;

                ModelState.AddModelError(string.Empty, errorMessage);
                var roles = _context.Roles.ToList();
                var clinics = _context.Clinics.ToList();
                ViewBag.Roles = new SelectList(roles, "RoleId", "Name");
                ViewBag.Clinics = new SelectList(clinics, "ClinicId", "ClinicAddress");
                return View(registerModel);
            }
        }

        public async Task<IActionResult> Login(string username, string password)
        {
            var apiUrl = "https://localhost:7044/api/EmployeeAPI/login";
            var loginModel = new Employee { EmployeeName = username, EmployeePassword = password };
            var content = new StringContent(JsonConvert.SerializeObject(loginModel), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();

                _contextAccessor.HttpContext.Session.SetString("AccessToken", token);

                var employee = await _context.Employees.FirstOrDefaultAsync(c => c.EmployeeName == username);

                _contextAccessor.HttpContext.Session.SetString("EmployeeName", employee.EmployeeName);
                if (employee.Avatar != null)
                {
                    _contextAccessor.HttpContext.Session.SetString("Avatar", employee.Avatar);
                }
                _contextAccessor.HttpContext.Session.SetString("EmployeeId", employee.EmployeeId.ToString());
                _contextAccessor.HttpContext.Session.SetString("Role", employee.RoleId.ToString());
                _contextAccessor.HttpContext.Session.SetString("EmployeeName", employee.EmployeeName);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                dynamic errorResponse = JsonConvert.DeserializeObject(errorContent);
                string errorMessage = errorResponse.message;
                ModelState.AddModelError(string.Empty, errorMessage);
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> ReloadEmployee(int employeeId)
        {
            var apiUrl = $"https://localhost:7044/api/EmployeeAPI/add/{employeeId}";
            var response = await _httpClient.PutAsync(apiUrl, null);

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

        public async Task<IActionResult> Delete(int employeeId)
        {
            if (HttpContext.Session.GetString("EmployeeId") == null)
            {
                HttpContext.Session.SetString("ReturnUrl", Url.Action("Delete", "Employee", new { employeeId }));

                return RedirectToAction("Login", "AdminView");
            }
            var apiUrl = $"https://localhost:7044/api/AdminApi/delete/{employeeId}";

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
