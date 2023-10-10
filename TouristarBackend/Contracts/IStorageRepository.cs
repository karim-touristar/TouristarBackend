namespace TouristarBackend.Contracts;

public interface IStorageRepository
{
    Task UploadFile(string bucketName, byte[] file, string fileName);
    Task<bool> DeleteFile(string bucketName, string fileName);
}
