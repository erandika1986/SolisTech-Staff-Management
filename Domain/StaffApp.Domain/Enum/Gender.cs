using System.ComponentModel;

namespace StaffApp.Domain.Enum
{
    public enum Gender
    {
        [Description("Male")]
        Male = 1,

        [Description("Female")]
        Female = 2,

        [Description("Unknown")]
        Unknown = 3
    }
}
