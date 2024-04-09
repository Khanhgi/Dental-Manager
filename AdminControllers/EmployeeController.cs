using Dental_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

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
            string createdBy = HttpContext.Session.GetString("EmployeeName");
            ViewBag.CreatedBy = createdBy;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Employee registerModel)
        {
            var apiUrl = $"https://localhost:7044/api/EmployeeAPI/register";

            registerModel.CreatedBy = HttpContext.Session.GetString("EmployeeName");

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

                if (employee.Avatar != null)
                {
                    _contextAccessor.HttpContext.Session.SetString("Avatar", employee.Avatar);
                }
                _contextAccessor.HttpContext.Session.SetString("EmployeeId", employee.EmployeeId.ToString());
                _contextAccessor.HttpContext.Session.SetString("Role", employee.RoleId.ToString());
                _contextAccessor.HttpContext.Session.SetString("EmployeeName", employee.EmployeeName);

                return RedirectToAction("Index", "Employee");
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
        public async Task<IActionResult> AddEmployee(int employeeId)
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

                return RedirectToAction("Index", "Employee");
            }
            var apiUrl = $"https://localhost:7044/api/EmployeeAPI/delete/{employeeId}";

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

        [HttpGet]
        public IActionResult Edit(int employeeId)
        {
            if (HttpContext.Session.GetString("EmployeeId") == null)
            {
                return RedirectToAction("Login", "Staff"); ////////////////////////////////////////////////////
            }
            var employee = _context.Employees
              .Include(s => s.EmployeeScheduleDetails)
              .ThenInclude(s => s.EmployeeSchedule)
              .FirstOrDefault(s => s.EmployeeId == employeeId);

            if (employee == null)
            {
                return NotFound();
            }

            var roles = _context.Roles.ToList();
            var clinics = _context.Clinics.ToList();

            ViewBag.Roles = new SelectList(roles, "RoleId", "Name");
            ViewBag.Clinics = new SelectList(clinics, "ClinicId", "ClinicAddress");

            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int employeeId, Employee updateModel)
        {
            if (!ModelState.IsValid)
            {
                var roles = _context.Roles.ToList();
                var clinics = _context.Clinics.ToList();

                ViewBag.Roles = new SelectList(roles, "RoleId", "Name");
                ViewBag.Clinics = new SelectList(clinics, "ClinicId", "ClinicAddress");

                return View(updateModel);
            }

            var phoneRegex = new Regex(@"^(03|05|07|08|09|01[2|6|8|9])(?!84)[0-9]{8}$");
            if (!phoneRegex.IsMatch(updateModel.EmployeePhone) || updateModel.EmployeePhone.Length > 10)
            {
                ModelState.AddModelError("EmployeePhone", "Invalid Phone number");
                return View(updateModel);
            }
            updateModel.Status = Request.Form["Status"] == "true";

            var apiUrl = $"https://localhost:7044/api/EmployeeAPI/update/{employeeId}";

            var json = JsonConvert.SerializeObject(updateModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var updatedEmployee = await _context.Employees.FirstOrDefaultAsync(s => s.EmployeeId == employeeId);
                    if (updatedEmployee != null)
                    {

                        string editorName = HttpContext.Session.GetString("EmployeeName");
                        updatedEmployee.UpdatedBy = editorName;

                        _context.Entry(updatedEmployee).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
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
                var roles = _context.Roles.ToList();
                var clinics = _context.Clinics.ToList();

                ViewBag.Roles = new SelectList(roles, "RoleId", "Name");
                ViewBag.Clinics = new SelectList(clinics, "ClinicId", "ClinicAddress");

                ModelState.AddModelError("", errorResponse.ToString());
                return View(updateModel);
            }
        }

        [HttpPost]
        public IActionResult ProcessUpload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Json("No file uploaded");
            }

            string fileName = Path.GetFileName(file.FileName);
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return Json("/images/" + fileName);
        }

        public async Task<IActionResult> Index()
        {
            var apiResponse = await _httpClient.GetAsync("https://localhost:7044/api/EmployeeAPI");

            if (apiResponse.IsSuccessStatusCode)
            {
                var responseContent = await apiResponse.Content.ReadAsStringAsync();
                var employee = JsonConvert.DeserializeObject<List<Employee>>(responseContent);
                return View(employee);
            }
            else
            {
                var employeeList= await _context.Employees
                    .Include(s => s.Clinic)
                    .Include(s => s.RoleId)
                    .ToListAsync();
                return View(employeeList);
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("EmployeeName");
            HttpContext.Session.Remove("Avatar");
            HttpContext.Session.Remove("Role");

            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }


    }
}
