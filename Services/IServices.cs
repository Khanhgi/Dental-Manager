using Dental_Manager.JWT_Token;
using DoAnT4.Services;

namespace DoAnT4n.Services
{
    public static class IServices
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<EmployeeServices>();
            services.AddScoped<GenerateToken>();
            return services;
        }
    }
}
