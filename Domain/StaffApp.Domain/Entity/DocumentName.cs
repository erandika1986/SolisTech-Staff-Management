using StaffApp.Domain.Entity.Common;
using StaffApp.Domain.Enum;

namespace StaffApp.Domain.Entity
{
    public class DocumentName : BaseEntity
    {
        public string Name { get; set; }
        public EmployeeDocumentCategory EmployeeDocumentCategory { get; set; }

        public virtual ICollection<UserQualificationDocument> UserQualificationDocuments { get; set; } = new HashSet<UserQualificationDocument>();
    }
}
