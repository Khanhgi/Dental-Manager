using Dental_Manager.JWT_Token;
using Dental_Manager.PatientApiController.Mail;
using Dental_Manager.PatientApiController.Services;
using Dental_Manager.Services;
using System.Reflection.Metadata;

namespace Dental_Manager.Services
{
    public static class IServices
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            //Admin
            services.AddHttpContextAccessor();
            services.AddScoped<EmployeeServices>();
            services.AddScoped<GenerateToken>();
            services.AddScoped<LoginEmployeeServices>();
            services.AddScoped<ClinicServices>();
            services.AddScoped<ServiceTypeServices>();

            //Patient
            services.AddScoped<AppoinmentServices>();
            services.AddScoped<AppoinmentDateServices>();
            services.AddScoped<SendMail>();
            return services;
        }
    }
}
