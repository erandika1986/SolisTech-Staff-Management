using System.ComponentModel;

namespace StaffApp.Domain.Enum
{
    public enum EmployeementType
    {
        [Description("Permanent")]
        Permanent = 1,

        [Description("Contract")]
        Contract = 2,

        [Description("Temporary")]
        Temporary = 3,

        [Description("Internship")]
        Internship = 4
    }
}
