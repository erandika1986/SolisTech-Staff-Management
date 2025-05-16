using StaffApp.Application.Services;

namespace StaffApp.Infrastructure.Services
{
    public class FileDownloadService : IFileDownloadService
    {
        //public List<string> GetAvailableFiles()
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<byte[]> GetFileAsync(string fullFilePath)
        {
            if (!File.Exists(fullFilePath))
            {
                throw new FileNotFoundException($"The file {fullFilePath} was not found.");
            }

            return await File.ReadAllBytesAsync(fullFilePath);
        }
    }
}
