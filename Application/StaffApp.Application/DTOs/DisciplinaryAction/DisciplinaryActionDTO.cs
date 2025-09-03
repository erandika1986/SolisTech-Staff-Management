using StaffApp.Application.DTOs.Common;
using StaffApp.Domain.Enum;

namespace StaffApp.Application.DTOs.DisciplinaryAction
{
    public class DisciplinaryActionDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DisciplinaryActionType ActionType { get; set; }
        public DropDownDTO SelectedActionType { get; set; }
        public DateTime? ActionDate { get; set; }

        public DateTime? EffectiveUntil { get; set; }
        public string Reason { get; set; }
        public SeverityLevel SeverityLevel { get; set; }
        public DropDownDTO SelectedSeverityLevel { get; set; }
        public string Remarks { get; set; }
    }
}
