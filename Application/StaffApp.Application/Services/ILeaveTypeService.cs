using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.Leave;

namespace StaffApp.Application.Services
{
    public interface ILeaveTypeService
    {
        Task<List<LeaveTypeDTO>> GetAllLeaveType();
        Task<GeneralResponseDTO> SaveLeaveType(LeaveTypeDTO model);
        Task<GeneralResponseDTO> DeleteLeaveType(int Id);
        Task<LeaveTypeDTO> GetLeaveTypeId(int id);
        Task<List<LeaveTypeConfigurationDTO>> GetLeaveTypeConfigurationId(int leaveTypeId);
        Task<GeneralResponseDTO> SaveLeaveTypeConfiguration(List<LeaveTypeConfigurationDTO> models);
    }
}
