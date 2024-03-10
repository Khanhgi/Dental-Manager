using Dental_Manager.Models;
using Dental_Manager.Services;
using DoAnT4.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoAnT4n.APIAdminController
{
    [ApiController]
    [Route("api/[controller")]
    public class EmployeeAPIController
    {
        private readonly QlkrContext _context;
        private readonly LoginEmployeeServices _loginEmployeeServices;
        private readonly EmployeeServices _employeeServices;

        public EmployeeAPIController(QlkrContext context, LoginEmployeeServices loginEmployeeServices, EmployeeServices employeeServices)
        {
            _context = context;
            _loginEmployeeServices = loginEmployeeServices;
            _employeeServices = employeeServices;
        }


    }
}
