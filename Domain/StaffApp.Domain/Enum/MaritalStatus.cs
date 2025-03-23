using System.ComponentModel;

namespace StaffApp.Domain.Enum
{
    public enum MaritalStatus
    {
        [Description("Single")]
        Single = 1,

        [Description("Married")]
        Married = 2,

        [Description("Widowed")]
        Widowed = 3,

        [Description("Divorced")]
        Divorced = 4,

        [Description("Separated")]
        Separated = 5
    }
}
