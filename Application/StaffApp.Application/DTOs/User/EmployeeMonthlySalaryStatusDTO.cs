using StaffApp.Domain.Enum;

namespace StaffApp.Application.DTOs.User
{
    public class EmployeeMonthlySalaryStatusDTO
    {
        public int Id { get; set; }
        public MonthlySalaryStatus Status { get; set; }
        public string StatusName { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedByUser { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedByUser { get; set; }
    }
}
