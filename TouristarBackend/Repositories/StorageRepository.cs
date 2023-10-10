using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using TouristarBackend.Contracts;

namespace TouristarBackend.Repositories;

public class StorageRepository : IStorageRepository
{
    private readonly StorageClient _client = StorageClient.Create(GetGoogleCredential());

    private static GoogleCredential GetGoogleCredential() =>
        GoogleCredential.FromFile(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "client_secrets.json")
        );

    public async Task<bool> DeleteFile(string bucketName, string fileName)
    {
        try
        {
            var storageObj = await _client.GetObjectAsync(bucketName, fileName);
            await _client.DeleteObjectAsync(storageObj);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task UploadFile(string bucketName, byte[] file, string fileName)
    {
        try
        {
            var result = await _client.UploadObjectAsync(
                bucketName,
                fileName,
                "image/jpeg",
                new MemoryStream(file)
            );
        }
        catch { }
    }
}
