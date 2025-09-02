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
        DbSet<MonthlySalary> MonthlySalaries { get; }
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
        DbSet<TaxLogic> TaxLogics { get; }
        DbSet<Project> Projects { get; }
        DbSet<ProjectDocument> ProjectDocuments { get; }
        DbSet<ProjectMember> ProjectMembers { get; }
        DbSet<TimeCard> TimeCards { get; }
        DbSet<TimeCardEntry> TimeCardEntries { get; }
        DbSet<Expense> Expenses { get; }
        DbSet<SupportAttachment> SupportAttachments { get; }
        DbSet<ExpenseType> ExpenseTypes { get; }
        DbSet<ExpenseSupportAttachment> ExpenseSupportAttachments { get; }
        DbSet<Income> Incomes { get; }
        DbSet<IncomeType> IncomeTypes { get; }
        DbSet<Invoice> Invoices { get; }
        DbSet<InvoiceDetail> InvoiceDetails { get; }
        DbSet<InvoicePayment> InvoicePayments { get; }
        DbSet<IncomeSupportAttachment> IncomeSupportAttachments { get; }

        DbSet<AppraisalPeriod> AppraisalPeriods { get; }
        DbSet<UserAppraisal> UserAppraisals { get; }
        DbSet<UserAppraisalCriteria> UserAppraisalCriterias { get; }
        DbSet<UserAppraisalDetail> UserAppraisalDetails { get; }
        DbSet<UserDisciplinaryAction> UserDisciplinaryActions { get; }
        DbSet<UserQualificationDocument> UserQualificationDocuments { get; }


        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task BeginTransactionAsync(CancellationToken cancellationToken);
        Task CommitTransactionAsync(CancellationToken cancellationToken);
        Task RollbackTransactionAsync(CancellationToken cancellationToken);
        Task RetryOnExceptionAsync(Func<Task> func);
    }
}
