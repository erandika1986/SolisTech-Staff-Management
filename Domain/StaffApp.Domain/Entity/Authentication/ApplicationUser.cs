﻿using Microsoft.AspNetCore.Identity;
using StaffApp.Domain.Enum;

namespace StaffApp.Domain.Entity.Authentication
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? NICNumber { get; set; }
        public string? LandNumber { get; set; }
        public DateTime? HireDate { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public MaritalStatus MaritalStatus { get; set; }
        public int? EmployeeTypeId { get; set; }
        public Gender? Gender { get; set; }
        public bool IsActive { get; set; }

        public virtual EmployeeType EmployeeType { get; set; }

        public virtual ICollection<Department> DepartmentHeads { get; set; } = new HashSet<Department>();

        public virtual ICollection<EmployeeSalary> EmployeeSalaries { get; set; } = new HashSet<EmployeeSalary>();
        public virtual ICollection<EmployeeLeaveRequest> EmployeeLeaveRequests { get; set; } = new HashSet<EmployeeLeaveRequest>();
        public virtual ICollection<EmployeeLeaveAllocation> EmployeeLeaveBalances { get; set; } = new HashSet<EmployeeLeaveAllocation>();

        //public virtual ICollection<EmployeeLeaveRequest> CreatedEmployeeLeaveRequests { get; set; } = new HashSet<EmployeeLeaveRequest>();
        //public virtual ICollection<EmployeeLeaveRequest> UpdatedEmployeeLeaveRequests { get; set; } = new HashSet<EmployeeLeaveRequest>();

        //public virtual ICollection<EmployeeLeaveRequestComment> CreatedEmployeeLeaveRequestComments { get; set; } = new HashSet<EmployeeLeaveRequestComment>();
        //public virtual ICollection<EmployeeLeaveRequestComment> UpdatedEmployeeLeaveRequestComments { get; set; } = new HashSet<EmployeeLeaveRequestComment>();

        //public virtual ICollection<EmployeeLeaveAllocation> CreatedEmployeeLeaveBalances { get; set; } = new HashSet<EmployeeLeaveAllocation>();
        //public virtual ICollection<EmployeeLeaveAllocation> UpdatedEmployeeLeaveBalances { get; set; } = new HashSet<EmployeeLeaveAllocation>();

        public virtual ICollection<EmployeeSalary> CreatedEmployeeSalaries { get; set; } = new HashSet<EmployeeSalary>();
        public virtual ICollection<EmployeeSalary> UpdatedEmployeeSalaries { get; set; } = new HashSet<EmployeeSalary>();

        //public virtual ICollection<EmployeeDepartment> EmployeeDepartments { get; set; } = new HashSet<EmployeeDepartment>();
        //public virtual ICollection<EmployeeDepartment> CreatedEmployeeDepartments { get; set; } = new HashSet<EmployeeDepartment>();
        //public virtual ICollection<EmployeeDepartment> UpdatedEmployeeDepartments { get; set; } = new HashSet<EmployeeDepartment>();

        //public virtual ICollection<LeaveTypeConfig> CreatedLeaveTypeConfigs { get; set; } = new HashSet<LeaveTypeConfig>();
        //public virtual ICollection<LeaveTypeConfig> UpdatedLeaveTypeConfigs { get; set; } = new HashSet<LeaveTypeConfig>();
    }
}
