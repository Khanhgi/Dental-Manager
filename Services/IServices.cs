using Dental_Manager.JWT_Token;
using Dental_Manager.Services;

namespace DoAnT4n.Services
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
            return services;
        }
    }
}
