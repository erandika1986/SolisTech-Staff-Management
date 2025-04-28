using System.ComponentModel;

namespace StaffApp.Domain.Enum
{
    public enum ProportionType
    {
        [Description("Percentage")]
        Percentage = 1,
        [Description("Fixed Amount")]
        FixedAmount = 2
    }
}
