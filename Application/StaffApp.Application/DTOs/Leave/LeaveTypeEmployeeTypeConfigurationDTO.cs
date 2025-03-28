namespace StaffApp.Application.DTOs.Leave
{
    public class LeaveTypeConfigurationContainerDTO
    {
        public List<LeaveTypeConfigurationDTO> LeaveTypeConfigurations { get; set; } = new List<LeaveTypeConfigurationDTO>();
    }

    public class LeaveTypeConfigurationDTO
    {
        public int Id { get; set; }
        public int EmployeeTypeId { get; set; }
        public string EmployeeTypeName { get; set; }
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; }
        public decimal AnnualLeaveAllowance { get; set; }
        public int MinimumServiceMonthsRequired { get; set; }
    }
}
