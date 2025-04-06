﻿using Microsoft.EntityFrameworkCore;
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
                await SeedCompanyLocationsAsync();
                await SeedAppSettingsAsync();
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
                    new Department { Name = "HR", CreatedDate = DateTime.Now, UpdateDate = DateTime.Now, CreatedByUserId = ApplicationConstants.EmptyGuide, UpdatedByUserId = ApplicationConstants.EmptyGuide,IsActive = true },
                    new Department { Name = "IT", CreatedDate = DateTime.Now, UpdateDate = DateTime.Now, CreatedByUserId = ApplicationConstants.EmptyGuide, UpdatedByUserId = ApplicationConstants.EmptyGuide,IsActive = true },
                    new Department { Name = "Finance", CreatedDate = DateTime.Now, UpdateDate = DateTime.Now, CreatedByUserId = ApplicationConstants.EmptyGuide, UpdatedByUserId = ApplicationConstants.EmptyGuide,IsActive = true },
                    new Department { Name = "Marketing", CreatedDate = DateTime.Now, UpdateDate = DateTime.Now, CreatedByUserId = ApplicationConstants.EmptyGuide, UpdatedByUserId = ApplicationConstants.EmptyGuide,IsActive = true },
                    new Department { Name = "Sales", CreatedDate = DateTime.Now, UpdateDate = DateTime.Now, CreatedByUserId = ApplicationConstants.EmptyGuide, UpdatedByUserId = ApplicationConstants.EmptyGuide,IsActive = true },
                    new Department { Name = "QA", CreatedDate = DateTime.Now, UpdateDate = DateTime.Now, CreatedByUserId = ApplicationConstants.EmptyGuide, UpdatedByUserId = ApplicationConstants.EmptyGuide,IsActive = true },
                    new Department { Name = "Development", CreatedDate = DateTime.Now, UpdateDate = DateTime.Now, CreatedByUserId = ApplicationConstants.EmptyGuide, UpdatedByUserId = ApplicationConstants.EmptyGuide,IsActive = true },
                    new Department { Name = "Management", CreatedDate = DateTime.Now, UpdateDate = DateTime.Now, CreatedByUserId = ApplicationConstants.EmptyGuide, UpdatedByUserId = ApplicationConstants.EmptyGuide,IsActive = true }
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
                        //All gender type eligible
                        AllowGenderType = ApplicationConstants.Zero,
                        IsActive = true,
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
                        ],
                        LeaveTypeAllowDurations =[
                            new LeaveTypeAllowDuration() { LeaveDuration = LeaveDuration.FullDay },
                            new LeaveTypeAllowDuration() { LeaveDuration = LeaveDuration.HalfDay }
                        ]
                    },
                    new LeaveType {
                        Name = "Sick Leave",
                        DefaultDays = 7,
                        HasLeaveTypeLogic = false,
                        IsActive = true,
                        //All gender type eligible
                        AllowGenderType = ApplicationConstants.Zero,
                        LeaveTypeConfigurations = [
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Permanent, AnnualLeaveAllowance = 7, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Contract, AnnualLeaveAllowance = 7, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Probation, AnnualLeaveAllowance = 7, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Temporary, AnnualLeaveAllowance = 7, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Internship, AnnualLeaveAllowance = 7, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Freelancer, AnnualLeaveAllowance = 0, MinimumServiceMonthsRequired = 0}
                        ],
                        LeaveTypeAllowDurations =[
                            new LeaveTypeAllowDuration() { LeaveDuration = LeaveDuration.FullDay },
                            new LeaveTypeAllowDuration() { LeaveDuration = LeaveDuration.HalfDay }
                        ]
                    },
                    new LeaveType {
                        Name = "Maternity Leave",
                        DefaultDays = 84 , HasLeaveTypeLogic = false,
                        AllowGenderType = (int)Gender.Female,
                        IsActive = true,
                        LeaveTypeConfigurations = [
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Permanent, AnnualLeaveAllowance = 84, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Contract, AnnualLeaveAllowance = 84, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Probation, AnnualLeaveAllowance = 84, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Temporary, AnnualLeaveAllowance = 84, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Internship, AnnualLeaveAllowance = 7, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Freelancer, AnnualLeaveAllowance = 0, MinimumServiceMonthsRequired = 0}
                        ],
                        LeaveTypeAllowDurations =[
                            new LeaveTypeAllowDuration() { LeaveDuration = LeaveDuration.FullDay }
                        ]
                    },
                    new LeaveType {
                        Name = "Paternity Leave",
                        DefaultDays = 0 ,
                        HasLeaveTypeLogic = false,
                        AllowGenderType = (int)Gender.Male,
                        IsActive = true,
                        LeaveTypeConfigurations = [
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Permanent, AnnualLeaveAllowance = 0, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Contract, AnnualLeaveAllowance = 0, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Probation, AnnualLeaveAllowance = 0, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Temporary, AnnualLeaveAllowance = 0, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Internship, AnnualLeaveAllowance = 0, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Freelancer, AnnualLeaveAllowance = 0, MinimumServiceMonthsRequired = 0}
                        ],
                        LeaveTypeAllowDurations =[
                            new LeaveTypeAllowDuration() { LeaveDuration = LeaveDuration.FullDay }
                        ]
                    },
                    new LeaveType {
                        Name = "NoPay Leave",
                        DefaultDays = 0 ,
                        HasLeaveTypeLogic = false,
                        //All gender type eligible
                        AllowGenderType = ApplicationConstants.Zero,
                        IsActive = true,
                        LeaveTypeConfigurations = [
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Permanent, AnnualLeaveAllowance = 0, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Contract, AnnualLeaveAllowance = 0, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Probation, AnnualLeaveAllowance = 0, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Temporary, AnnualLeaveAllowance = 0, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Internship, AnnualLeaveAllowance = 0, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Freelancer, AnnualLeaveAllowance = 0, MinimumServiceMonthsRequired = 0}
                        ],
                        LeaveTypeAllowDurations =[
                            new LeaveTypeAllowDuration() { LeaveDuration = LeaveDuration.FullDay },
                            new LeaveTypeAllowDuration() { LeaveDuration = LeaveDuration.HalfDay }
                        ]
                    }
,
                    new LeaveType {
                        Name = "Short Leave",
                        DefaultDays = 0 ,
                        HasLeaveTypeLogic = true,
                        IsActive = true,
                        //All gender type eligible
                        AllowGenderType = ApplicationConstants.Zero,
                        LeaveTypeConfigurations = [
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Permanent, AnnualLeaveAllowance = 0, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Contract, AnnualLeaveAllowance = 0, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Probation, AnnualLeaveAllowance = 0, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Temporary, AnnualLeaveAllowance = 0, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Internship, AnnualLeaveAllowance = 0, MinimumServiceMonthsRequired = 0},
                            new LeaveTypeConfig(){ EmployeeTypeId = (int)EmploymentType.Freelancer, AnnualLeaveAllowance = 0, MinimumServiceMonthsRequired = 0}
                        ],
                        LeaveTypeAllowDurations =[
                            new LeaveTypeAllowDuration() { LeaveDuration = LeaveDuration.ShortLeave }
                        ]

                    }
                };
                await context.LeaveTypes.AddRangeAsync(leaveTypes);
                await context.SaveChangesAsync();
            }
        }
        private async Task SeedCompanyLocationsAsync()
        {
            if (!context.CompanyLocations.Any())
            {
                await context.CompanyLocations.AddRangeAsync(new List<CompanyLocation>()
               {
                   new CompanyLocation(){ Name = "Solistech - Sri Lanka Office", Country="Sri Lanka",Address="No 59-2/1, Sunethradevi rd,Kohuwala, Sri Lanka",TimeZone = "Asia/Colombo", CreatedDate=DateTime.Now,UpdateDate = DateTime.Now, IsActive = true },
                   new CompanyLocation(){ Name = "Solistech - USA Office", Country="United State",Address="3725 Wexford Hollow Rd E Jacksonville, Florida, USA",TimeZone ="Eastern Daylight Time", CreatedDate=DateTime.Now,UpdateDate = DateTime.Now, IsActive = true },
               });

                await context.SaveChangesAsync();
            }
        }

        private async Task SeedAppSettingsAsync()
        {
            if (!context.AppSettings.Any())
            {
                var appSettings = new List<AppSetting>
                {
                    new AppSetting() { Name = "SMTPServer", Value = "" },
                    new AppSetting() { Name = "SMTPPort", Value = "" },
                    new AppSetting() { Name = "SMTPUsername", Value = "" },
                    new AppSetting() { Name = "SMTPPassword", Value = "" },
                    new AppSetting() { Name = "SMTPEnableSsl", Value = "" },
                    new AppSetting() { Name = "SMTPSenderEmail", Value = "" },
                    new AppSetting() { Name = "ApplicationUrl", Value = "" },
                    new AppSetting() { Name = "CompanyName", Value = "" },
                    new AppSetting() { Name = "CompanyLogoUrl", Value = "" },
                    new AppSetting() { Name = "CompanyAddress", Value = "" },
                    new AppSetting() { Name = "LeaveRequestCCList", Value = "" }

                };
                await context.AppSettings.AddRangeAsync(appSettings);

                await context.SaveChangesAsync();
            }
        }
    }
}
