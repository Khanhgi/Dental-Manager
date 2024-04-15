using Dental_Manager.JWT_Token;
using Dental_Manager.PatientApiController.Mail;
using Dental_Manager.PatientApiController.Services;
using Dental_Manager.Services;

namespace Dental_Manager.Services
{
    public static class IServices
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<EmployeeServices>();
            services.AddScoped<GenerateToken>();
            services.AddScoped<LoginEmployeeServices>();
            //services.AddScoped<DoctorServices>();
            services.AddScoped<ClinicServices>();
            services.AddScoped<AppoinmentServices>();
            services.AddScoped<SendMail>();
            return services;
        }
    }
}
