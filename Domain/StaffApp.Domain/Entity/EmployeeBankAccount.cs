using StaffApp.Domain.Entity.Authentication;
using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class EmployeeBankAccount : BaseAuditableEntity
    {
        public string EmployeeId { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string BranchName { get; set; }
        public string BranchCode { get; set; }
        public bool IsPrimaryAccount { get; set; }

        public virtual ApplicationUser Employee { get; set; }

    }
}
