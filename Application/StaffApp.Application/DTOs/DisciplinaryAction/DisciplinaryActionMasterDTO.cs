using StaffApp.Application.DTOs.Common;

namespace StaffApp.Application.DTOs.DisciplinaryAction
{
    public class DisciplinaryActionMasterDTO
    {
        public List<DropDownDTO> ActionTypes { get; set; } = new();
        public List<DropDownDTO> SeverityLevels { get; set; } = new();
    }
}
