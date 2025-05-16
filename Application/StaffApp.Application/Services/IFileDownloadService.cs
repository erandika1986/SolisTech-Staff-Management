namespace StaffApp.Application.Services
{
    public interface IFileDownloadService
    {
        Task<byte[]> GetFileAsync(string fullFilePath);
        //List<string> GetAvailableFiles();
    }
}
