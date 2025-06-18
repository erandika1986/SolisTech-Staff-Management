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

        [Display(Name = "Created By")]
        public string CreatedByUser { get; set; }

        [Display(Name = "Created Date")]
        public string CreatedOn { get; set; }

        [Display(Name = "Updated User")]
        public string UpdatedByUser { get; set; }

        [Display(Name = "Updated Date")]
        public string UpdatedOn { get; set; }

        public List<Microsoft.AspNetCore.Components.Forms.IBrowserFile> Files { get; set; } = new List<Microsoft.AspNetCore.Components.Forms.IBrowserFile> { };
        public List<SupportAttachmentDTO> SavedSupportFiles { get; set; } = new List<SupportAttachmentDTO>();
    }
}
