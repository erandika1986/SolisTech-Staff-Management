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
                await SeedCompanyLocationsAsync();
                await SeedAppSettingsAsync();
                await SeedSalaryAddonAsync();
                await SeedExpensesTypeAsync();
                await SeedIncomeTypeAsync();
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
                    await roleService.CreateRoleAsync(roleName, false, 0.0m);
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
                    new AppSetting() { Name = CompanySettingConstants.SMTPServer, Value = "" },
                    new AppSetting() { Name = CompanySettingConstants.SMTPPort, Value = "" },
                    new AppSetting() { Name = CompanySettingConstants.SMTPUsername, Value = "" },
                    new AppSetting() { Name = CompanySettingConstants.SMTPPassword, Value = "" },
                    new AppSetting() { Name = CompanySettingConstants.SMTPEnableSsl, Value = "" },
                    new AppSetting() { Name = CompanySettingConstants.SMTPSenderEmail, Value = "" },
                    new AppSetting() { Name = CompanySettingConstants.ApplicationUrl, Value = "" },
                    new AppSetting() { Name = CompanySettingConstants.CompanyEmail, Value = "" },
                    new AppSetting() { Name = CompanySettingConstants.CompanyPhone, Value = "" },
                    new AppSetting() { Name = CompanySettingConstants.CompanyName, Value = "" },
                    new AppSetting() { Name = CompanySettingConstants.CompanyLogoUrl, Value = "" },
                    new AppSetting() { Name = CompanySettingConstants.CompanyAddress, Value = "" },
                    new AppSetting() { Name = CompanySettingConstants.LeaveRequestCCList, Value = "" },
                    new AppSetting() { Name = CompanySettingConstants.IsPasswordLoginEnable, Value = "False"},
                    new AppSetting() { Name = CompanySettingConstants.SalarySlipFolderPath, Value = "C:\\WordDocuments\\"}

                };
                await context.AppSettings.AddRangeAsync(appSettings);

                await context.SaveChangesAsync();
            }
        }

        private async Task SeedSalaryAddonAsync()
        {
            if (!context.SalaryAddons.Any())
            {
                var salaryAddons = new List<SalaryAddon>
                {
                    new SalaryAddon() { Name = "PAYE", AddonType = SalaryAddonType.Deduction , ApplyForAllEmployees = true, ProportionType = ProportionType.Percentage, DefaultValue = 0 },
                    new SalaryAddon() { Name = "EPF", AddonType = SalaryAddonType.Deduction , ApplyForAllEmployees = true, ProportionType = ProportionType.Percentage, DefaultValue = 8 },
                    new SalaryAddon() { Name = "Stamp Duty", AddonType = SalaryAddonType.Deduction , ApplyForAllEmployees = true, ProportionType = ProportionType.FixedAmount, DefaultValue = 0 },
                    new SalaryAddon() { Name = "Welfare", AddonType = SalaryAddonType.Deduction , ApplyForAllEmployees = true, ProportionType = ProportionType.FixedAmount, DefaultValue = 0 },
                    new SalaryAddon() { Name = "Other Deductions", AddonType = SalaryAddonType.Deduction , ApplyForAllEmployees = true, ProportionType = ProportionType.FixedAmount, DefaultValue = 0 },
                    new SalaryAddon() { Name = "EPF", AddonType = SalaryAddonType.SocialSecuritySchemesEmployeeShare , ApplyForAllEmployees = true, ProportionType = ProportionType.Percentage, DefaultValue = 12 },
                    new SalaryAddon() { Name = "ETF", AddonType = SalaryAddonType.SocialSecuritySchemesEmployeeShare , ApplyForAllEmployees = true, ProportionType = ProportionType.Percentage, DefaultValue = 3 },
                    new SalaryAddon() { Name = "Motor Vehicle", AddonType = SalaryAddonType.Allowance , ApplyForAllEmployees = false, ProportionType = ProportionType.FixedAmount, DefaultValue = 0 },
                    new SalaryAddon() { Name = "Performance", AddonType = SalaryAddonType.Allowance , ApplyForAllEmployees = false, ProportionType = ProportionType.FixedAmount  , DefaultValue = 0 },
                    new SalaryAddon() { Name = "House Rent Allowance (HRA)", AddonType = SalaryAddonType.Allowance , ApplyForAllEmployees = false, ProportionType = ProportionType.FixedAmount  , DefaultValue = 0 },
                    new SalaryAddon() { Name = "Conveyance Allowance", AddonType = SalaryAddonType.Allowance , ApplyForAllEmployees = false, ProportionType = ProportionType.FixedAmount  , DefaultValue = 0 },
                    new SalaryAddon() { Name = "Medical Allowance", AddonType = SalaryAddonType.Allowance , ApplyForAllEmployees = false, ProportionType = ProportionType.FixedAmount  , DefaultValue = 0 },
                    new SalaryAddon() { Name = "Leave Travel Allowance (LTA)", AddonType = SalaryAddonType.Allowance , ApplyForAllEmployees = false, ProportionType = ProportionType.FixedAmount  , DefaultValue = 0 },
                    new SalaryAddon() { Name = "City Compensatory Allowance (CCA)", AddonType = SalaryAddonType.Allowance , ApplyForAllEmployees = false, ProportionType = ProportionType.FixedAmount  , DefaultValue = 0 },
                    new SalaryAddon() { Name = "Children Education Allowance", AddonType = SalaryAddonType.Allowance , ApplyForAllEmployees = false, ProportionType = ProportionType.FixedAmount  , DefaultValue = 0 },
                    new SalaryAddon() { Name = "Uniform Allowance", AddonType = SalaryAddonType.Allowance , ApplyForAllEmployees = false, ProportionType = ProportionType.FixedAmount  , DefaultValue = 0 },
                    new SalaryAddon() { Name = "Fuel Allowance", AddonType = SalaryAddonType.Allowance , ApplyForAllEmployees = false, ProportionType = ProportionType.FixedAmount  , DefaultValue = 0 },
                    new SalaryAddon() { Name = "Driver  Allowance", AddonType = SalaryAddonType.Allowance , ApplyForAllEmployees = false, ProportionType = ProportionType.FixedAmount  , DefaultValue = 0 },
                };

                await context.SalaryAddons.AddRangeAsync(salaryAddons);
                await context.SaveChangesAsync();
            }
        }

        private async Task SeedIncomeTypeAsync()
        {
            if (!context.IncomeTypes.Any())
            {
                var incomeTypes = new List<IncomeType>
                {
                    new IncomeType(){ Name ="Product Sales" },
                    new IncomeType(){ Name ="Service Income" },
                    new IncomeType(){ Name ="Subscription Fees" },
                    new IncomeType(){ Name ="Licensing Fees" },
                    new IncomeType(){ Name ="Project-Based Income" },
                    new IncomeType(){ Name ="Membership Fees" },
                    new IncomeType(){ Name ="Interest Income" },
                    new IncomeType(){ Name ="Dividend Income" },
                    new IncomeType(){ Name ="Rental Income" },
                    new IncomeType(){ Name ="Gain From Asset Sales" },
                    new IncomeType(){ Name ="Forex Gains" },
                    new IncomeType(){ Name ="Refunds" },
                    new IncomeType(){ Name ="Grants" },
                    new IncomeType(){ Name ="Subsidies" },
                    new IncomeType(){ Name ="Commission Income" },
                    new IncomeType(){ Name ="Donations" }
                };

                await context.IncomeTypes.AddRangeAsync(incomeTypes);
                await context.SaveChangesAsync();
            }
        }

        private async Task SeedExpensesTypeAsync()
        {
            if (!context.ExpenseTypes.Any())
            {
                var expenseTypes = new List<ExpenseType>
                {
                    new ExpenseType(){ Name ="Salaries and Wages" },
                    new ExpenseType(){ Name ="Employee Benefits (EPF/ETF, Insurance, Bonuses)" },
                    new ExpenseType(){ Name ="Rent" },
                    new ExpenseType(){ Name ="Utilities" },
                    new ExpenseType(){ Name ="Office Supplies and Stationery" },
                    new ExpenseType(){ Name ="Repairs and Maintenance" },
                    new ExpenseType(){ Name ="Marketing and Advertising" },
                    new ExpenseType(){ Name ="Travel and Accommodation" },
                    new ExpenseType(){ Name ="Training and Development" },
                    new ExpenseType(){ Name ="Software Subscriptions" },
                    new ExpenseType(){ Name ="Legal and Professional Fees" },
                    new ExpenseType(){ Name ="Accounting or Audit Fees" },
                    new ExpenseType(){ Name ="Bank Charges and Service Fees" },
                    new ExpenseType(){ Name ="Telephone and Communication" },
                    new ExpenseType(){ Name ="Depreciation and Amortization" },
                    new ExpenseType(){ Name ="Insurance (property, liability, etc.)" },
                    new ExpenseType(){ Name ="Raw Materials" },
                    new ExpenseType(){ Name ="Packaging and shipping" },
                    new ExpenseType(){ Name ="Manufacturing labor" },
                    new ExpenseType(){ Name ="Equipment usage (factory, tools)" },
                    new ExpenseType(){ Name ="Inventory purchases" },
                    new ExpenseType(){ Name ="Loan interest" },
                    new ExpenseType(){ Name ="Credit card processing fees" },
                    new ExpenseType(){ Name ="Lease payments" },
                    new ExpenseType(){ Name ="Bad debts" },
                    new ExpenseType(){ Name ="Income tax" },
                    new ExpenseType(){ Name ="VAT/Sales tax" },
                    new ExpenseType(){ Name ="Withholding tax" },
                    new ExpenseType(){ Name ="Property tax (if applicable)" }
                };

                await context.ExpenseTypes.AddRangeAsync(expenseTypes);
                await context.SaveChangesAsync();
            }
        }
    }
}
