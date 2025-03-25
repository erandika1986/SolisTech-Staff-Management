using Microsoft.EntityFrameworkCore;
using StaffApp.Domain.Entity;
using StaffApp.Domain.Entity.Authentication;

namespace StaffApp.Application.Contracts
{
    public interface IStaffAppDbContext
    {
        DbSet<ApplicationUser> ApplicationUsers { get; }
        DbSet<CompanyFinancialYear> CompanyFinancialYears { get; }
        DbSet<CompanyYear> CompanyYears { get; }
        DbSet<Department> Departments { get; }
        DbSet<EmployeeDepartment> EmployeeDepartments { get; }
        DbSet<EmployeeLeaveAllocation> EmployeeLeaveAllocations { get; }
        DbSet<EmployeeLeaveRequest> EmployeeLeaveRequests { get; }
        DbSet<EmployeeLeaveRequestComment> EmployeeLeaveRequestComments { get; }
        DbSet<EmployeeSalary> EmployeeSalaries { get; }
        DbSet<EmployeeType> EmployeeTypes { get; }
        DbSet<LeaveType> LeaveTypes { get; }
        DbSet<LeaveTypeConfig> LeaveTypesConfigs { get; }
        DbSet<LeaveTypeLogic> LeaveTypesLogics { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task BeginTransactionAsync(CancellationToken cancellationToken);
        Task CommitTransactionAsync(CancellationToken cancellationToken);
        Task RollbackTransactionAsync(CancellationToken cancellationToken);
        Task RetryOnExceptionAsync(Func<Task> func);
    }
}
