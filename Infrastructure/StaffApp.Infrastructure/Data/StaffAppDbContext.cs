using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using StaffApp.Application.Contracts;
using StaffApp.Domain.Entity;
using StaffApp.Domain.Entity.Authentication;
using StaffApp.Infrastructure.Interceptors;
using System.Diagnostics;
using System.Reflection;

namespace StaffApp.Infrastructure.Data
{
    public class StaffAppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>, IStaffAppDbContext
    {
        private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;
        private IDbContextTransaction dbContextTransaction;

        public StaffAppDbContext(
            DbContextOptions<StaffAppDbContext> options,
            AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor) : base(options)
        {
            this._auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(message => Debug.WriteLine(message))
                .EnableSensitiveDataLogging();
            optionsBuilder.UseLazyLoadingProxies();

            if (_auditableEntitySaveChangesInterceptor != null)
                optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            dbContextTransaction ??= await Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken)
        {
            try
            {
                await SaveChangesAsync(cancellationToken);
                dbContextTransaction?.CommitAsync(cancellationToken);
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                if (dbContextTransaction != null)
                {
                    DisposeTransaction();
                }
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
        {
            try
            {
                await dbContextTransaction?.RollbackAsync(cancellationToken);
            }
            finally
            {
                DisposeTransaction();
            }
        }

        public async Task RetryOnExceptionAsync(Func<Task> func)
        {
            await Database.CreateExecutionStrategy().ExecuteAsync(func);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        private void DisposeTransaction()
        {
            try
            {
                if (dbContextTransaction != null)
                {
                    dbContextTransaction.Dispose();
                    dbContextTransaction = null;
                }
            }
            catch (Exception ex)
            {

            }
        }

        public DbSet<CompanyFinancialYear> CompanyFinancialYears => Set<CompanyFinancialYear>();
        public DbSet<CompanyLocation> CompanyLocations => Set<CompanyLocation>();
        public DbSet<CompanyYear> CompanyYears => Set<CompanyYear>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<EmployeeLeaveAllocation> EmployeeLeaveAllocations => Set<EmployeeLeaveAllocation>();
        public DbSet<EmployeeLeaveRequest> EmployeeLeaveRequests => Set<EmployeeLeaveRequest>();
        public DbSet<MonthlySalary> MonthlySalaries => Set<MonthlySalary>();
        public DbSet<EmployeeSalary> EmployeeSalaries => Set<EmployeeSalary>();
        public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();
        public DbSet<EmployeeDepartment> EmployeeDepartments => Set<EmployeeDepartment>();
        public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
        public DbSet<EmployeeLeaveRequestComment> EmployeeLeaveRequestComments => Set<EmployeeLeaveRequestComment>();
        public DbSet<EmployeeType> EmployeeTypes => Set<EmployeeType>();
        public DbSet<LeaveTypeConfig> LeaveTypesConfigs => Set<LeaveTypeConfig>();
        public DbSet<LeaveTypeLogic> LeaveTypesLogics => Set<LeaveTypeLogic>();
        public DbSet<EmployeeLeaveRequestSupportFile> EmployeeLeaveRequestSupportFiles => Set<EmployeeLeaveRequestSupportFile>();
        public DbSet<LeaveTypeAllowDuration> LeaveTypeAllowDurations => Set<LeaveTypeAllowDuration>();
        public DbSet<AppSetting> AppSettings => Set<AppSetting>();
        public DbSet<EmployeeBankAccount> EmployeeBankAccounts => Set<EmployeeBankAccount>();
        public DbSet<SalaryAddon> SalaryAddons => Set<SalaryAddon>();
        public DbSet<EmployeeSalaryAddon> EmployeeSalaryAddons => Set<EmployeeSalaryAddon>();
        public DbSet<EmployeeMonthlySalary> EmployeeMonthlySalaries => Set<EmployeeMonthlySalary>();
        public DbSet<EmployeeMonthlySalaryAddon> EmployeeMonthlySalaryAddons => Set<EmployeeMonthlySalaryAddon>();
        public DbSet<EmployeeSalaryHistory> EmployeeSalaryHistories => Set<EmployeeSalaryHistory>();
        public DbSet<EmployeeSalaryAddonHistory> EmployeeSalaryAddonHistories => Set<EmployeeSalaryAddonHistory>();
        public DbSet<TaxLogic> TaxLogics => Set<TaxLogic>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<ProjectDocument> ProjectDocuments => Set<ProjectDocument>();
        public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
        public DbSet<TimeCard> TimeCards => Set<TimeCard>();
        public DbSet<TimeCardEntry> TimeCardEntries => Set<TimeCardEntry>();
        public DbSet<Expense> Expenses => Set<Expense>();
        public DbSet<ExpenseType> ExpenseTypes => Set<ExpenseType>();
        public DbSet<Income> Incomes => Set<Income>();
        public DbSet<IncomeType> IncomeTypes => Set<IncomeType>();
        public DbSet<SupportAttachment> SupportAttachments => Set<SupportAttachment>();
        public DbSet<ExpenseSupportAttachment> ExpenseSupportAttachments => Set<ExpenseSupportAttachment>();
        public DbSet<IncomeSupportAttachment> IncomeSupportAttachments => Set<IncomeSupportAttachment>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<InvoiceDetail> InvoiceDetails => Set<InvoiceDetail>();
        public DbSet<InvoicePayment> InvoicePayments => Set<InvoicePayment>();

        public DbSet<AppraisalPeriod> AppraisalPeriods => Set<AppraisalPeriod>();
        public DbSet<UserAppraisal> UserAppraisals => Set<UserAppraisal>();
        public DbSet<UserAppraisalCriteria> UserAppraisalCriterias => Set<UserAppraisalCriteria>();
        public DbSet<UserAppraisalDetail> UserAppraisalDetails => Set<UserAppraisalDetail>();
        public DbSet<UserDisciplinaryAction> UserDisciplinaryActions => Set<UserDisciplinaryAction>();
        public DbSet<UserQualificationDocument> UserQualificationDocuments => Set<UserQualificationDocument>();
    }
}
