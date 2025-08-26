namespace StaffApp.Application.Services
{
    public interface IAppLogger
    {
        void LogError(Exception ex, string message = "");
    }
}
