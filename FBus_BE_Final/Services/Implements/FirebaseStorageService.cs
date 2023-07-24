using Google.Cloud.Storage.V1;

namespace FBus_BE.Services
{
    public class FirebaseStorageService : IFirebaseStorageService   
    {
        private readonly StorageClient _storageClient;
        private const string BucketName = "fbus-388009.appspot.com";

        public FirebaseStorageService()
        {
            _storageClient = StorageClient.Create();
        }

        public async Task<Uri> UploadFile(string name, IFormFile file, string type)
        {
            //Guid randomGuid = Guid.NewGuid();
            //using MemoryStream stream = new MemoryStream();
            //await file.CopyToAsync(stream);
            //var blob = await _storageClient.UploadObjectAsync(BucketName, type + "/" +
            //    $"{name}-{randomGuid}", file.ContentType, stream);
            //var photoUri = new Uri(blob.MediaLink);
            //return photoUri;
            return null;
        }

        public async Task DeleteFile(string name)
        {
            //await _storageClient.DeleteObjectAsync(BucketName, name);
        }
    }
}
