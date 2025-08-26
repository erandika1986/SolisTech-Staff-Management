using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class CompanyYear : BaseEntity
    {
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsCurrentYear { get; set; }

        public DateTime? CreatedDate { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? UpdatedByUserId { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<EmployeeLeaveAllocation> EmployeeLeaveBalances { get; set; } = new HashSet<EmployeeLeaveAllocation>();
        public virtual ICollection<EmployeeLeaveRequest> EmployeeLeaveRequests { get; set; } = new HashSet<EmployeeLeaveRequest>();
        public virtual ICollection<MonthlySalary> MonthlySalaries { get; set; } = new HashSet<MonthlySalary>();
        public virtual ICollection<Invoice> Invoices { get; set; } = new HashSet<Invoice>();
        public virtual ICollection<Expense> Expenses { get; set; } = new HashSet<Expense>();
    }
}
