using StaffApp.Application.DTOs.Common;

namespace StaffApp.Application.DTOs.Finance
{
    public class ExpenseMasterDataDTO
    {
        public List<DropDownDTO> ExpenseTypes { get; set; } = new();
        public List<DropDownDTO> CompanyYears { get; set; } = new();
        public List<DropDownDTO> Months { get; set; } = new();
    }
}
