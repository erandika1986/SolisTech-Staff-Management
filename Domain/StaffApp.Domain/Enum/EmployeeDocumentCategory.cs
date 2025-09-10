using System.ComponentModel;

namespace StaffApp.Domain.Enum
{
    public enum EmployeeDocumentCategory
    {
        [Description("Mandatory Identification & Legal Documents")]
        MandatoryIdentificationAndLegalDocuments = 1,

        [Description("Employment Records")]
        EmploymentRecords = 2,

        [Description("Qualifications & Experience")]
        QualificationsAndExperience = 3,

        [Description("Compliance & Policy Documents")]
        ComplianceAndPolicyDocuments = 4,

        [Description("Health & Safety Documents")]
        HealthAndSafetyRecords = 5
    }
}
