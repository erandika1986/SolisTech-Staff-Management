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
        public DbSet<CompanyYear> CompanyYears => Set<CompanyYear>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<EmployeeLeaveAllocation> EmployeeLeaveAllocations => Set<EmployeeLeaveAllocation>();
        public DbSet<EmployeeLeaveRequest> EmployeeLeaveRequests => Set<EmployeeLeaveRequest>();
        public DbSet<EmployeeSalary> EmployeeSalaries => Set<EmployeeSalary>();
        public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();
        public DbSet<EmployeeDepartment> EmployeeDepartments => Set<EmployeeDepartment>();
        public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
        public DbSet<EmployeeLeaveRequestComment> EmployeeLeaveRequestComments => Set<EmployeeLeaveRequestComment>();
        public DbSet<EmployeeType> EmployeeTypes => Set<EmployeeType>();
        public DbSet<LeaveTypeConfig> LeaveTypesConfigs => Set<LeaveTypeConfig>();
        public DbSet<LeaveTypeLogic> LeaveTypesLogics => Set<LeaveTypeLogic>();
    }
}
