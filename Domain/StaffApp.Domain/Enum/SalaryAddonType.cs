using System.ComponentModel;

namespace StaffApp.Domain.Enum
{
    public enum SalaryAddonType
    {
        [Description("Allowance")]
        Allowance = 1,
        [Description("Deduction")]
        Deduction = 2,
        [Description("Social Security Schemes Employee Share")]
        SocialSecuritySchemesEmployeeShare = 3,
        [Description("Social Security Schemes Company Share")]
        SocialSecuritySchemesCompanyShare = 4
    }
}
