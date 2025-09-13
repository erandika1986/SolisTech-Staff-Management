using StaffApp.Domain.Entity.Common;
using StaffApp.Domain.Entity.Salary;

namespace StaffApp.Domain.Entity
{
    public class TaxLogic : BaseEntity
    {
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public decimal TaxRate { get; set; }

        public int SalaryAddonId { get; set; }

        public virtual SalaryAddon SalaryAddon { get; set; }
    }
}
