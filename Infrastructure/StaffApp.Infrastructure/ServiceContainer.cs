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

            services.AddTransient<IStaffAppDbContext>(provider => provider.GetRequiredService<StaffAppDbContext>());

            services.AddTransient<StaffAppDbContextInitializer>();

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<StaffAppDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            services.AddTransient<IDateTime, DateTimeService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IDepartmentService, DepartmentService>();

            return services;
        }
    }
}
