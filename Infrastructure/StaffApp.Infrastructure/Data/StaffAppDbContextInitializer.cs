using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StaffApp.Application.Extensions.Constants;
using StaffApp.Application.Extensions.Helpers;
using StaffApp.Application.Services;
using StaffApp.Domain.Entity;
using StaffApp.Domain.Enum;

namespace StaffApp.Infrastructure.Data
{
    public class StaffAppDbContextInitializer
    {
        private readonly StaffAppDbContext context;
        private readonly IRoleService roleService;
        private readonly ILogger<StaffAppDbContextInitializer> logger;
        public StaffAppDbContextInitializer(StaffAppDbContext context,
            IRoleService roleService,
            ILogger<StaffAppDbContextInitializer> logger)
        {
            this.context = context;
            this.logger = logger;
            this.roleService = roleService;
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
                await SeedEmployeeTypeAsync();
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
                RoleConstants.TraineeSoftwareEngineer,
                RoleConstants.TraineeQAEngineer
            };

            foreach (var roleName in roles)
            {
                if (!await roleService.RoleExistsAsync(roleName))
                {
                    await roleService.CreateRoleAsync(roleName, false);
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

        private async Task SeedEmployeeTypeAsync()
        {
            var employeeTypes = EnumHelper.GetDropDownList<EmploymentType>();

            foreach (var employeeType in employeeTypes)
            {
                if (!context.EmployeeTypes.Any(x => x.Id == employeeType.Id))
                {
                    context.EmployeeTypes.Add(new EmployeeType() { Id = employeeType.Id, Name = employeeType.Name });
                }
            }
            await context.SaveChangesAsync();
        }

        private async Task SeedLeaveTypeAsync()
        {
            if (!context.LeaveTypes.Any())
            {
                var leaveTypes = new List<LeaveType>
                {
                    new LeaveType
                    {
                        Name = "Annual Leave",
                        DefaultDays = 14 ,
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
                        ],
                        LeaveTypeConfigurations = [
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Permanent, AnnualLeaveAllowance = 14, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Contract, AnnualLeaveAllowance = 14, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Probation, AnnualLeaveAllowance = 7, MinimumServiceMonthsRequired = 6 },
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Temporary, AnnualLeaveAllowance = 14, MinimumServiceMonthsRequired = 3},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Internship, AnnualLeaveAllowance = 0, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Freelancer, AnnualLeaveAllowance = 0, MinimumServiceMonthsRequired = 0}
                        ]
                    },
                    new LeaveType {
                        Name = "Sick Leave",
                        DefaultDays = 7,
                        HasLeaveTypeLogic = false,
                        LeaveTypeConfigurations = [
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Permanent, AnnualLeaveAllowance = 7, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Contract, AnnualLeaveAllowance = 7, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Probation, AnnualLeaveAllowance = 7, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Temporary, AnnualLeaveAllowance = 7, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Internship, AnnualLeaveAllowance = 7, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Freelancer, AnnualLeaveAllowance = 0, MinimumServiceMonthsRequired = 0}
                        ]
                    },
                    new LeaveType {
                        Name = "Maternity Leave",
                        DefaultDays = 84 , HasLeaveTypeLogic = false,
                        LeaveTypeConfigurations = [
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Permanent, AnnualLeaveAllowance = 84, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Contract, AnnualLeaveAllowance = 84, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Probation, AnnualLeaveAllowance = 84, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Temporary, AnnualLeaveAllowance = 84, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Internship, AnnualLeaveAllowance = 7, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Freelancer, AnnualLeaveAllowance = 0, MinimumServiceMonthsRequired = 0}
                        ]
                    },
                    new LeaveType {
                        Name = "Paternity Leave",
                        DefaultDays = 0 ,
                        HasLeaveTypeLogic = false,
                        LeaveTypeConfigurations = [
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Permanent, AnnualLeaveAllowance = 0, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Contract, AnnualLeaveAllowance = 0, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Probation, AnnualLeaveAllowance = 0, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Temporary, AnnualLeaveAllowance = 0, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Internship, AnnualLeaveAllowance = 0, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Freelancer, AnnualLeaveAllowance = 0, MinimumServiceMonthsRequired = 0}
                        ]

                    }
                };
                await context.LeaveTypes.AddRangeAsync(leaveTypes);
                await context.SaveChangesAsync();
            }
        }
    }
}
