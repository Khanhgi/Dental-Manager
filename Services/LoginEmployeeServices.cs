using Dental_Manager.JWT_Token;
using Dental_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace Dental_Manager.Services
{
    public class LoginEmployeeServices
    {
        private readonly QlkrContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly GenerateToken _generateToken;

        public LoginEmployeeServices(QlkrContext context, IHttpContextAccessor contextAccessor, GenerateToken generateToken)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _generateToken = generateToken;
        }

        public async Task<IActionResult> RegisterEmployee(Employee registerModel)
        {
            if (registerModel == null)
            {
                return new BadRequestObjectResult("Không tìm thấy dữ liệu");
            }

            if (string.IsNullOrWhiteSpace(registerModel.EmployeeName) || string.IsNullOrWhiteSpace(registerModel.EmployeeEmail) || string.IsNullOrWhiteSpace(registerModel.EmployeePhone) || string.IsNullOrWhiteSpace(registerModel.EmployeePassword))
            {
                var emptyFieldsErrorResponse = new
                {
                    Message = "Vui lòng nhập đầy đủ thông tin !"
                };
                return new BadRequestObjectResult(emptyFieldsErrorResponse);
            }
            if (registerModel.RoleId == null || registerModel.ClinicId == null)
            {
                var emptyIdFieldsResponse = new
                {
                    Message = "Vui lòng nhập ID của quyền hạn và ID bệnh viện !",
                };
                return new BadRequestObjectResult(emptyIdFieldsResponse);

            }

            var createEmployee = await _context.Employee
                .Include(e => e.Role)
                .FirstOrDefaultAsync(p => p.EmployeeId == registerModel.EmployeeId || p.EmployeeName == registerModel.EmployeeName);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerModel.EmployeePassword);

            var newEmployee = new Employee
            {
                EmployeeName = registerModel.EmployeeName,
                EmployeePassword = hashedPassword,
                EmployeeEmail = registerModel.EmployeeEmail,
                EmployeePhone = registerModel.EmployeePhone,
                EmployeeAddress = registerModel.EmployeeAddress,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = registerModel.CreatedBy,
                Status = registerModel.Status,
                IsDeleted = false,
                Role = createEmployee?.Role,
                Clinic = createEmployee?.Clinic
            };
            if (registerModel.RoleId != null)
            {
                var role = await _context.Roles.FindAsync(registerModel.RoleId);
                if (role == null)
                {
                    return new BadRequestObjectResult("ID quyền hạn được cung cấp không hợp lệ");
                }
                newEmployee.Role = role;
            }

            if (registerModel.ClinicId != null)
            {
                var clinic = await _context.Clinics.FindAsync(registerModel.ClinicId);
                if (clinic == null)
                {
                    return new BadRequestObjectResult("ID bệnh viện được cung cấp không hợp lệ");
                }
                newEmployee.Clinic = clinic;
            }

            _context.Employee.Add(newEmployee);
            await _context.SaveChangesAsync();

            _context.Entry(newEmployee).Reference(c => c.Clinic).Load();
            _context.Entry(newEmployee).Reference(c => c.Role).Load();

            var registerSuccessResponse = new
            {
                Message = "Đăng kí thành công",
                EmployeeId = newEmployee.EmployeeId,
                Role = new
                {
                    Name = newEmployee.Role?.Name,
                    RoleId = newEmployee.Role?.RoleId
                }
            };
            return new OkObjectResult(registerSuccessResponse);
        }


        public async Task<IActionResult> Login(Employee loginModel)
        {
            if (loginModel == null || string.IsNullOrWhiteSpace(loginModel.EmployeeName) || string.IsNullOrWhiteSpace(loginModel.EmployeePassword))
            {
                var errorResponse = new
                {
                    Message = "Tên và Mật khẩu không được để trống !",
                };
                return new BadRequestObjectResult(errorResponse);
            }

            var employee = await _context.Employee.FirstOrDefaultAsync(c => c.EmployeeName == loginModel.EmployeeName);

            if (employee == null)
            {
                var loginErrorResponse = new
                {
                    Message = "Không tìm thấy tài khoản",
                };
                return new BadRequestObjectResult(loginErrorResponse);
            }

            if (employee.Status == false)
            {
                var inactiveAccountResponse = new
                {
                    Message = "Tài khoản chưa được kích hoạt"
                };
                return new BadRequestObjectResult(inactiveAccountResponse);
            }

            if (employee.FailedLoginAttempt >= 5 && employee.LastFailedLoginAttempt != null)
            {
                var timeSinceLastFailedAttempt = DateTime.Now - employee.LastFailedLoginAttempt.Value;
                if (timeSinceLastFailedAttempt.TotalMinutes <= 5)
                {
                    var lockAccountResponse = new
                    {
                        Message = "Tài khoản của bạn bị khoá 5 phút vì đã nhập sai 5 lần !",
                    };
                    return new BadRequestObjectResult(lockAccountResponse);
                }
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginModel.EmployeePassword, employee.EmployeePassword);

            if (isPasswordValid)
            {
                employee.FailedLoginAttempt = 0;
                employee.LastFailedLoginAttempt = null;
                _context.SaveChanges();

                string token = _generateToken.CreateToken(employee);

                // Lấy HttpContext từ IHttpContextAccessor
                var httpContext = _contextAccessor.HttpContext;

                httpContext.Session.SetString("Username", employee.EmployeeName);
                // Lưu thông tin vào session
                if (employee.Avatar != null)
                {
                    httpContext.Session.SetString("Avatar", employee.Avatar);
                }
                httpContext.Session.SetString("EmployeeId", employee.EmployeeId.ToString());
                httpContext.Session.SetString("Role", employee.RoleId.ToString());
                httpContext.Session.SetString("EmployeeName", employee.EmployeeName);

                var loginSuccessResponse = new
                {
                    Token = token,
                    Message = "Đăng nhập thành công"
                };

                return new OkObjectResult(loginSuccessResponse);
            }
            else
            {
                employee.FailedLoginAttempt++;
                employee.LastFailedLoginAttempt = DateTime.Now;
                _context.SaveChanges();

                var invalidLoginErrorResponse = new
                {
                    Message = "Tên hoặc Mật khẩu không hợp lệ !",
                    Errors = new List<string>
                    {
                        "Mật khẩu không hợp lệ"
                    }
                };
                return new BadRequestObjectResult(invalidLoginErrorResponse);
            }
        }

        public async Task<IActionResult> GetEmployeeInfoById(int employeeId)
        {
            if (employeeId <= 0)
            {
                return new BadRequestObjectResult("Mã nhân viên không hợp lệ");
            }

            var employee = await _context.Employee.FindAsync(employeeId);

            if (employee == null)
            {
                return new NotFoundObjectResult("Không tìm thấy nhân viên");
            }

            var employeeInfo = new
            {
                EmployeeId = employee.EmployeeId,
                EmployeeName = employee.EmployeeName,
                EmployeeEmail = employee.EmployeeEmail,
                EmployeePhone = employee.EmployeePhone,
                EmployeeAvatar = employee.Avatar,

            };

            return new OkObjectResult(employeeInfo);
        }
    }

}
