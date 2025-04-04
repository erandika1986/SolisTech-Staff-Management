using Google.Apis.Calendar.v3;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StaffApp.Application.Contracts;
using StaffApp.Application.Services;
using StaffApp.Domain.Entity.Authentication;
using StaffApp.Infrastructure.Data;
using StaffApp.Infrastructure.Interceptors;
using StaffApp.Infrastructure.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<AuditableEntitySaveChangesInterceptor>();

            services.AddDbContext<StaffAppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<LoadingService>();

            services.AddTransient<CalendarService>();

            services.AddTransient<IStaffAppDbContext>(provider => provider.GetRequiredService<StaffAppDbContext>());

            services.AddTransient<StaffAppDbContextInitializer>();

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<StaffAppDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();


            //Register all custom build services
            var InterfaceAssembly = typeof(IUserService).Assembly;
            var classAssembly = typeof(UserService).Assembly;

            // Find all interfaces
            var interfaces = InterfaceAssembly.GetTypes()
                .Where(t => t.IsInterface && t.Namespace == "StaffApp.Application.Services");



            foreach (var interfaceType in interfaces)
            {
                // Find the implementation(s) of each interface
                var implementations = classAssembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && interfaceType.IsAssignableFrom(t));

                foreach (var implementationType in implementations)
                {
                    // Register as transient service
                    services.AddTransient(interfaceType, implementationType);
                }
            }

            services.AddTransient<IDateTime, DateTimeService>();

            return services;
        }
    }
}
