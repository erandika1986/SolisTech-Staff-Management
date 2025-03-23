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
        DbSet<EmployeeLeaveApproval> EmployeeLeaveApprovals { get; }
        DbSet<EmployeeLeaveBalance> EmployeeLeaveBalances { get; }
        DbSet<EmployeeLeaveRequest> EmployeeLeaveRequests { get; }
        DbSet<EmployeeSalary> EmployeeSalaries { get; }
        DbSet<LeaveType> LeaveTypes { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task BeginTransactionAsync(CancellationToken cancellationToken);
        Task CommitTransactionAsync(CancellationToken cancellationToken);
        Task RollbackTransactionAsync(CancellationToken cancellationToken);
        Task RetryOnExceptionAsync(Func<Task> func);
    }
}
