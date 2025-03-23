using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StaffApp.Application.Extensions.Constants;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Data
{
    public class StaffAppDbContextInitializer
    {
        private readonly StaffAppDbContext context;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ILogger<StaffAppDbContextInitializer> logger;
        public StaffAppDbContextInitializer(StaffAppDbContext context,
            RoleManager<IdentityRole> roleManager,
            ILogger<StaffAppDbContextInitializer> logger)
        {
            this.context = context;
            this.logger = logger;
            this.roleManager = roleManager;
        }

        public async Task InitializeAsync()
        {
            try
            {
                if (context.Database.IsSqlServer())
                {
                    await context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await SeedUserRolesAsync();
                await SeedDepartmentAsync();
                await SeedLeaveTypeAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private async Task SeedUserRolesAsync()
        {

            string[] roles =
            {
                RoleConstants.Admin,
                RoleConstants.Director,
                RoleConstants.Manager,
                RoleConstants.TeamLead,
                RoleConstants.SeniorSoftwareEngineer,
                RoleConstants.SoftwareEngineer,
                RoleConstants.AssociateSoftwareEngineer,
                RoleConstants.SeniorQAEngineer,
                RoleConstants.QAEngineer,
                RoleConstants.AssociateQAEngineer,
                RoleConstants.Intern
            };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private async Task SeedDepartmentAsync()
        {
            if (!context.Departments.Any())
            {
                var departments = new List<Department>
                {
                    new Department { Name = "HR" },
                    new Department { Name = "IT" },
                    new Department { Name = "Finance" },
                    new Department { Name = "Marketing" },
                    new Department { Name = "Sales" },
                    new Department { Name = "QA" },
                    new Department { Name = "Development" },
                    new Department { Name = "Management" }
                };

                await context.Departments.AddRangeAsync(departments);
                await context.SaveChangesAsync();
            }
        }

        private async Task SeedLeaveTypeAsync()
        {
            if (!context.LeaveTypes.Any())
            {
                var leaveTypes = new List<LeaveType>
                {
                    new LeaveType { Name = "Annual Leave",DefaultDays = 14 ,HasLeaveTypeLogic = false},
                    new LeaveType { Name = "Sick Leave", DefaultDays = 7, HasLeaveTypeLogic = false },
                    new LeaveType { Name = "Maternity Leave", DefaultDays = 84 , HasLeaveTypeLogic = false},
                    new LeaveType
                    {
                        Name = "Paternity Leave",
                        DefaultDays = 0 ,
                        HasLeaveTypeLogic = true,
                        LeaveTypeLogics =
                        [
                            new LeaveTypeLogic()
                            {
                                StartMonthOfYear = 1,
                                StartDateOfMonth = 1,
                                EndMonthOfYear = 3,
                                EndDateOfMonth = 31,
                                EntitledLeaveCount = 14
                            },
                            new LeaveTypeLogic()
                            {
                                StartMonthOfYear = 4,
                                StartDateOfMonth = 1,
                                EndMonthOfYear = 6,
                                EndDateOfMonth = 30,
                                EntitledLeaveCount = 10
                            },
                            new LeaveTypeLogic()
                            {
                                StartMonthOfYear = 7,
                                StartDateOfMonth = 1,
                                EndMonthOfYear = 9,
                                EndDateOfMonth = 30,
                                EntitledLeaveCount = 7
                            },
                            new LeaveTypeLogic()
                            {
                                StartMonthOfYear = 10,
                                StartDateOfMonth = 1,
                                EndMonthOfYear = 12,
                                EndDateOfMonth = 31,
                                EntitledLeaveCount = 4
                            }
                        ]
                    }
                };
                await context.LeaveTypes.AddRangeAsync(leaveTypes);
                await context.SaveChangesAsync();
            }
        }
    }
}
