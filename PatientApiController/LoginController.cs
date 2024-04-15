using Dental_Manager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Dental_Manager.ClientApiController
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class LoginController : Controller
    {
        private readonly QlkrContext _context;

        public LoginController(QlkrContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(Patient loginModel)
        {
            if (string.IsNullOrWhiteSpace(loginModel.PatientName) || string.IsNullOrWhiteSpace(loginModel.PatientPassword))
            {
                var errorResponse = new
                {
                    Message = "Username and password cannot be empty"
                };
                return BadRequest(errorResponse);
            }

            var patient = await _context.Patients.FirstOrDefaultAsync(c => c.PatientName == loginModel.PatientName);
            if (patient == null)
            {
                var loginErrorResponse = new
                {
                    Message = "wrong pass or name",
                };
                return BadRequest(loginErrorResponse);
            }

            var passwordHasher = new PasswordHasher<Patient>();
            var result = passwordHasher.VerifyHashedPassword(null, patient.PatientPassword, loginModel.PatientPassword);

            if (result == PasswordVerificationResult.Success)
            {
                HttpContext.Session.SetString("PatientId", patient.PatientId.ToString());
                var loginSuccessResponse = new
                {
                    PatientID = patient.PatientId,
                    Name = patient.PatientName,
                    phone = patient.PatientPhone,
                    Address = patient.PatientAddress,
                    Email = patient.PatientEmail,
                    Message = "Login successful"
                };
                return Ok(loginSuccessResponse);
            }

            var invalidLoginErrorResponse = new
            {
                Message = "wrong pass or usenmae",
                Errors = new List<string>
                {
                    "Invalid password"
                }
            };

            return BadRequest(invalidLoginErrorResponse);
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(Patient registrationModel)
        {
            if (registrationModel == null)
            {
                return BadRequest("Registration data is empty.");
            }

            if (string.IsNullOrWhiteSpace(registrationModel.PatientName) ||
                string.IsNullOrWhiteSpace(registrationModel.PatientPassword) ||
                string.IsNullOrWhiteSpace(registrationModel.PatientPhone) ||
                string.IsNullOrWhiteSpace(registrationModel.PatientEmail))
            {
                var emptyFieldsErrorResponse = new
                {
                    Message = "Không được để trống username password name phone Email đâu Cưng!",
                };
                return BadRequest(emptyFieldsErrorResponse);
            }

            var vietnamesePhoneNumberPattern = @"^(0[0-9]{9,10})$";
            if (!Regex.IsMatch(registrationModel.PatientPhone, vietnamesePhoneNumberPattern))
            {
                var phoneFormatErrorResponse = new
                {
                    Message = "Vui lòng nhập đúng định dạng số điện thoại",
                };
                return BadRequest(phoneFormatErrorResponse);
            }

            var emailRegex = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
            if (!Regex.IsMatch(registrationModel.PatientEmail, emailRegex))
            {
                var emailEmailFormat = new
                {
                    Message = "Vui lòng nhập đúng định dạng Email",
                };
                return BadRequest(emailEmailFormat);
            }

            if (ModelState.IsValid)
            {
                var passwordHasher = new PasswordHasher<Patient>();
                var hashedPassword = passwordHasher.HashPassword(null, registrationModel.PatientPassword);

                int defaultRoleId = 4;

                var role = await _context.Roles.FindAsync(defaultRoleId);

                if (role == null)
                {
                    return Ok("Default role not found.");
                }
                var newClient = new Patient
                {
                    PatientName = registrationModel.PatientName,
                    PatientPassword = hashedPassword,
                    PatientPhone = registrationModel.PatientPhone,
                    PatientAddress = registrationModel.PatientAddress,
                    PatientEmail = registrationModel.PatientEmail,
                    Status = registrationModel.Status,
                    Role = role,
                };

                _context.Patients.Add(newClient);
                await _context.SaveChangesAsync();

                _context.Entry(newClient).Reference(c => c.Role).Load();

                var registrationSuccessResponse = new
                {
                    Message = "Registration successful",
                    PatientId = newClient.PatientId,
                    Role = new
                    {
                        Name = newClient.Role?.Name,
                        RoleId = newClient.Role?.RoleId
                    }
                };
                return Ok(registrationSuccessResponse);
            }

            var invalidDataErrorResponse = new
            {
                Message = "Invalid registration data",
                Errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            };
            return BadRequest(invalidDataErrorResponse);
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetPatientById(int? id)
        {
            if (id == null)
            {
                return Ok(null);
            }
            var user = await _context.Patients.FindAsync(id);

            if (user == null)
            {
                return NotFound("Patient not found");
            }

            return Ok(user);
        }

        [HttpPut("update/{patientId}")]
        public async Task<IActionResult> patientId(int patientId, Patient updateModel)
        {
            var client = await _context.Patients
                .FirstOrDefaultAsync(c => c.PatientId == patientId);

            if (client == null)
            {
                return NotFound();
            }

            client.PatientName = updateModel.PatientName;
            client.PatientPhone = updateModel.PatientPhone;
            client.PatientAddress= updateModel.PatientAddress;

            if (updateModel.RoleId != client.RoleId)
            {
                var newRole = await _context.Roles.FindAsync(updateModel.RoleId);
                if (newRole != null)
                {
                    client.Role = newRole;
                }
            }
            _context.Entry(client).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var updateSuccessResponse = new
            {
                Message = "Patient updated successfully",
                Name = client.PatientName,
                Phone = client.PatientPhone,
                Address = client.PatientPhone,
                Email = client.PatientEmail
            };

            return Ok(updateSuccessResponse);
        }
    }
}
