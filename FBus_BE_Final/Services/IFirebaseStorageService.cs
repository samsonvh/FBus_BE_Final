namespace FBus_BE.Services
{
    public interface IFirebaseStorageService
    {
        Task<Uri> UploadFile(string name, IFormFile file, string type);
        Task DeleteFile(string name);
    }
}
