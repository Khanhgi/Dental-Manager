using Dental_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace Dental_Manager.AdminControllers
{
    public class ScheduleDetailController : Controller
    {
        QlkrContext qlkr = new QlkrContext();
        private readonly HttpClient _httpClient;

        public ScheduleDetailController()
        {
            _httpClient = new HttpClient();
        }


        public async Task<IActionResult> Index()
        {
            var apiResponse = await _httpClient.GetAsync("https://localhost:7044/api/ScheduleDetailApi/");
            if (apiResponse.IsSuccessStatusCode)
            {
                var responseContent = await apiResponse.Content.ReadAsStringAsync();
                var schedules = JsonConvert.DeserializeObject<List<EmployeeScheduleDetail>>(responseContent);

                return View(schedules);
            }

            ViewBag.ErrorMessage = "Failed to retrieve schedule data from the API.";
            return View();
        }

        public IActionResult Create()
        {

            var employeeList = qlkr.Employees.ToList();
            var scheduleList = qlkr.EmployeeSchedules.ToList();

            ViewBag.EmployeeId = new SelectList(employeeList, "EmployeeId", "EmployeeName");
            ViewBag.EmployeeScheduleId = new SelectList(scheduleList, "EmployeeScheduleId", "Time");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeScheduleDetail inputModel)
        {
            try
            {

                if (inputModel.EmployeeId == 0 || inputModel.EmployeeScheduleId == 0)
                {
                    ModelState.AddModelError("", "Please select a Employee and a Schedule.");
                    var employeeList = await qlkr.Employees.ToListAsync();
                    var scheduleList = await qlkr.EmployeeSchedules.ToListAsync();
                    ViewBag.EmployeeId = employeeList.Select(s => new SelectListItem { Value = s.EmployeeId.ToString(), Text = s.EmployeeName });
                    ViewBag.EmployeeScheduleId = scheduleList.Select(s => new SelectListItem { Value = s.EmployeeScheduleId.ToString(), Text = s.Time.ToString() });
                    return View(inputModel);
                }

                if (!inputModel.Date.HasValue)
                {
                    ModelState.AddModelError("", "Please provide a valid Date.");
                    var employeeList = await qlkr.Employees.ToListAsync();
                    var scheduleList = await qlkr.EmployeeSchedules.ToListAsync();
                    ViewBag.EmployeeId = employeeList.Select(s => new SelectListItem { Value = s.EmployeeId.ToString(), Text = s.EmployeeName });
                    ViewBag.EmployeeScheduleId = scheduleList.Select(s => new SelectListItem { Value = s.EmployeeScheduleId.ToString(), Text = s.Time.ToString() });
                    return View(inputModel);
                }


                var selectedEmployee = await qlkr.Employees.FindAsync(inputModel.EmployeeId);
                var selectedSchedule = await qlkr.EmployeeSchedules.FindAsync(inputModel.EmployeeScheduleId);

                inputModel.Employee = selectedEmployee;
                inputModel.EmployeeSchedule = selectedSchedule;

                var existingDetail = await qlkr.EmployeeScheduleDetails
                    .FirstOrDefaultAsync(sd =>
                        sd.EmployeeId == inputModel.EmployeeId &&
                        sd.EmployeeScheduleId == inputModel.EmployeeScheduleId);

                if (existingDetail != null)
                {
                    ModelState.AddModelError("", "A schedule detail already exists for the selected Staff and Schedule.");
                    var employeeList = await qlkr.Employees.ToListAsync();
                    var scheduleList = await qlkr.EmployeeSchedules.ToListAsync();
                    ViewBag.EmployeeId = employeeList.Select(s => new SelectListItem { Value = s.EmployeeId.ToString(), Text = s.EmployeeName });
                    ViewBag.EmployeeScheduleId = scheduleList.Select(s => new SelectListItem { Value = s.EmployeeScheduleId.ToString(), Text = s.Time.ToString() });
                    return View(inputModel);
                }

                var serializedModel = JsonConvert.SerializeObject(inputModel);
                var content = new StringContent(serializedModel, Encoding.UTF8, "application/json");

                var apiResponse = await _httpClient.PostAsync("https://localhost:7044/api/ScheduleDetailApi/create", content);

                if (apiResponse.IsSuccessStatusCode)
                {
                    var responseContent = await apiResponse.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<EmployeeScheduleDetail>(responseContent);
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorResponse = await apiResponse.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", "Failed to create Scheduledetail: " + errorResponse);
                    var employeeList = await qlkr.Employees.ToListAsync();
                    var scheduleList = await qlkr.EmployeeSchedules.ToListAsync();
                    ViewBag.EmployeeId = employeeList.Select(s => new SelectListItem { Value = s.EmployeeId.ToString(), Text = s.EmployeeName });
                    ViewBag.EmployeeScheduleId = scheduleList.Select(s => new SelectListItem { Value = s.EmployeeScheduleId.ToString(), Text = s.Time.ToString() });
                    return View(inputModel);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpGet]
        public IActionResult Edit(int scheduleId, int employeeId)
        {

            var employeeList = qlkr.Employees.ToList();
            var scheduleList = qlkr.EmployeeSchedules.ToList();

            ViewBag.EmployeeList = new SelectList(employeeList, "EmployeeId", "EmployeeName");
            ViewBag.ScheduleList = new SelectList(scheduleList, "EmployeeScheduleId", "Time");

            var scheduleDetail = qlkr.EmployeeScheduleDetails
                .FirstOrDefault(sd => sd.EmployeeScheduleId == scheduleId && sd.EmployeeId == employeeId);

            if (scheduleDetail == null)
            {
                return NotFound();
            }

            return View(scheduleDetail);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int scheduleId, int employeeId, EmployeeScheduleDetail updatedModel)
        {
            try
            {
                updatedModel.EmployeeId = employeeId;
                updatedModel.EmployeeScheduleId = scheduleId;

                var employee = await qlkr.Employees.FindAsync(employeeId);
                var schedule = await qlkr.EmployeeSchedules.FindAsync(scheduleId);

                if (employee != null && schedule != null)
                {
                    updatedModel.Employee = employee;
                    updatedModel.EmployeeSchedule = schedule;

                    var apiUrl = $"https://localhost:7044/api/ScheduleDetailApi/update?scheduleId={scheduleId}&employeeId={employeeId}";

                    var serializedModel = JsonConvert.SerializeObject(updatedModel);
                    var content = new StringContent(serializedModel, Encoding.UTF8, "application/json");

                    var apiResponse = await _httpClient.PutAsync(apiUrl, content);

                    if (apiResponse.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var errorResponse = await apiResponse.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", "Failed to update Scheduledetail: " + errorResponse);

                        var employeeList = qlkr.Employees.ToList();
                        var scheduleList = qlkr.EmployeeSchedules.ToList();
                        ViewBag.EmployeeId = new SelectList(employeeList, "EmployeeId", "EmployeeName");
                        ViewBag.EmployeeScheduleId = new SelectList(scheduleList, "EmployeeScheduleId", "Time");

                        return View(updatedModel);
                    }
                }
                else
                {
                    return NotFound("Employee or Schedule not found.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        public async Task<IActionResult> Delete(int employeeId, int scheduleId)
        {
            try
            {
                var apiUrl = $"https://localhost:7044/api/ScheduleDetailApi/delete?employeeId={employeeId}&scheduleId={scheduleId}";

                var apiResponse = await _httpClient.DeleteAsync(apiUrl);

                if (apiResponse.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorResponse = await apiResponse.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", "Failed to delete Scheduledetail: " + errorResponse);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }

    }
}
