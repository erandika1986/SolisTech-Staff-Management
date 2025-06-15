using StaffApp.Application.DTOs.Common;

namespace StaffApp.Application.DTOs.Finance
{
    public class ExpenseDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string DateName { get; set; }
        public decimal Amount { get; set; }
        public string Notes { get; set; }
        public int ExpenseTypeId { get; set; }
        public string ExpenseType { get; set; }

        public List<Microsoft.AspNetCore.Components.Forms.IBrowserFile> Files { get; set; } = new List<Microsoft.AspNetCore.Components.Forms.IBrowserFile> { };
        public List<SupportAttachmentDTO> SavedSupportFiles { get; set; } = new List<SupportAttachmentDTO>();
    }
}
