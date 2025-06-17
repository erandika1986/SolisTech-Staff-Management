using StaffApp.Application.DTOs.Common;

namespace StaffApp.Application.DTOs.Finance
{
    public class ExpenseDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string DateName { get; set; }
        public double Amount { get; set; }
        public string Notes { get; set; }
        public DropDownDTO ExpenseType { get; set; }
        public string ExpenseTypeName { get; set; }

        public List<Microsoft.AspNetCore.Components.Forms.IBrowserFile> Files { get; set; } = new List<Microsoft.AspNetCore.Components.Forms.IBrowserFile> { };
        public List<SupportAttachmentDTO> SavedSupportFiles { get; set; } = new List<SupportAttachmentDTO>();
    }
}
