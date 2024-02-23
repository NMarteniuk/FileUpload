namespace FileUploadApp.Services
{
    public interface IFileService
    {
        Task<string> UploadBlobAsync(string fileName, string contentType, Stream fileStream, string email);
        Task<string> GetSASToken(string fileName);
    }
}
