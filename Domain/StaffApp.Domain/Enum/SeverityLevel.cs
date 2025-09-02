using System.ComponentModel;

namespace StaffApp.Domain.Enum
{
    public enum SeverityLevel
    {
        [Description("Low")]
        Low = 1,

        [Description("Medium")]
        Medium = 2,

        [Description("High")]
        High = 3,

        [Description("Critical")]
        Critical = 4
    }
}
