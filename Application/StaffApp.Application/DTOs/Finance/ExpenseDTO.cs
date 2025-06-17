using StaffApp.Application.DTOs.Common;
using System.ComponentModel.DataAnnotations;

namespace StaffApp.Application.DTOs.Finance
{
    public class ExpenseDTO
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        public DateTime Date { get; set; }


        [Display(Name = "Date")]
        public string DateName { get; set; }


        [Display(Name = "Amount")]
        public double Amount { get; set; }

        [Display(Name = "Note")]
        public string Notes { get; set; }

        public DropDownDTO ExpenseType { get; set; }


        [Display(Name = "Expense Type")]
        public string ExpenseTypeName { get; set; }

        public List<Microsoft.AspNetCore.Components.Forms.IBrowserFile> Files { get; set; } = new List<Microsoft.AspNetCore.Components.Forms.IBrowserFile> { };
        public List<SupportAttachmentDTO> SavedSupportFiles { get; set; } = new List<SupportAttachmentDTO>();
    }
}
