using System.ComponentModel;

namespace StaffApp.Domain.Enum
{
    public enum ProjectManagementPlatform
    {
        [Description("Jira")]
        Jira = 1,
        [Description("Azure DevOps")]
        AzureDevOps = 2,
        [Description("Trello")]
        Trello = 3,
        [Description("Click Up")]
        ClickUp = 4
    }
}
