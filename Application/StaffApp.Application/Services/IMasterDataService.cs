using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.User;

namespace StaffApp.Application.Services
{
    public interface IMasterDataService
    {
        Task<List<DropDownDTO>> GetAvailableCompanyYears();
        List<DropDownDTO> GetAvailableMonths();
        Task<List<DropDownDTO>> GetAvailableEmploymentTypes();
        List<DropDownDTO> GetAvailableGenderTypes();
        List<DropDownDTO> GetAllowGenderTypes();
        Task<List<RoleDTO>> GetAvailableRoles();
        Task<List<DropDownDTO>> GetLeaveTypes(bool hasDefaultValue = false);
        List<DropDownDTO> GetLeaveStatuses(bool hasDefaultValue = false);
        Task<List<DropDownDTO>> GetLeaveDurations(int leaveTypeId);
        List<DropDownDTO> GetHalfDaySessionTypes();
        List<DropDownDTO> GetShortLeaveSessionTypes();
        Task<List<DropDownDTO>> GetOfficeLocations();
        List<DropDownDTO> GetEmployeeSalaryStatus(bool hasDefaultValue = false);
        List<DropDownDTO> GetEmployeeSalaryTransferStatus();
    }
}
