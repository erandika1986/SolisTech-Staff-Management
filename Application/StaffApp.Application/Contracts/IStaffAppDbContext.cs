using Microsoft.EntityFrameworkCore;
using StaffApp.Domain.Entity;
using StaffApp.Domain.Entity.Authentication;

namespace StaffApp.Application.Contracts
{
    public interface IStaffAppDbContext
    {
        DbSet<SalaryAddon> SalaryAddons { get; }
        DbSet<AppSetting> AppSettings { get; }
        DbSet<ApplicationUser> ApplicationUsers { get; }
        DbSet<CompanyFinancialYear> CompanyFinancialYears { get; }
        DbSet<CompanyLocation> CompanyLocations { get; }
        DbSet<CompanyYear> CompanyYears { get; }
        DbSet<Department> Departments { get; }
        DbSet<EmployeeMonthlySalary> EmployeeMonthlySalaries { get; }
        DbSet<EmployeeMonthlySalaryAddon> EmployeeMonthlySalaryAddons { get; }
        DbSet<EmployeeDepartment> EmployeeDepartments { get; }
        DbSet<EmployeeLeaveAllocation> EmployeeLeaveAllocations { get; }
        DbSet<EmployeeLeaveRequest> EmployeeLeaveRequests { get; }
        DbSet<EmployeeLeaveRequestSupportFile> EmployeeLeaveRequestSupportFiles { get; }
        DbSet<EmployeeLeaveRequestComment> EmployeeLeaveRequestComments { get; }
        DbSet<EmployeeSalaryAddon> EmployeeSalaryAddons { get; }
        DbSet<EmployeeBankAccount> EmployeeBankAccounts { get; }
        DbSet<EmployeeSalary> EmployeeSalaries { get; }
        DbSet<EmployeeSalaryHistory> EmployeeSalaryHistories { get; }
        DbSet<EmployeeSalaryAddonHistory> EmployeeSalaryAddonHistories { get; }
        DbSet<EmployeeType> EmployeeTypes { get; }
        DbSet<LeaveType> LeaveTypes { get; }
        DbSet<LeaveTypeAllowDuration> LeaveTypeAllowDurations { get; }
        DbSet<LeaveTypeConfig> LeaveTypesConfigs { get; }
        DbSet<LeaveTypeLogic> LeaveTypesLogics { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task BeginTransactionAsync(CancellationToken cancellationToken);
        Task CommitTransactionAsync(CancellationToken cancellationToken);
        Task RollbackTransactionAsync(CancellationToken cancellationToken);
        Task RetryOnExceptionAsync(Func<Task> func);
    }
}
