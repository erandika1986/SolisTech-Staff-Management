using System.ComponentModel;

namespace StaffApp.Domain.Enum
{
    public enum DisciplinaryActionType
    {
        [Description("Warning")]
        Warning = 1,

        [Description("Suspension")]
        Suspension = 2,

        [Description("Demotion")]
        Demotion = 3,

        [Description("Termination")]
        Termination = 4,

        [Description("Other")]
        Other = 5
    }
}
