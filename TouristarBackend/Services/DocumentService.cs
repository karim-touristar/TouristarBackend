using TouristarModels.Models;
using TouristarBackend.Contracts;
using TouristarModels.Constants;

namespace TouristarBackend.Services;

public class DocumentService : IDocumentService
{
    private readonly IRepositoryManager _repository;

    public DocumentService(IRepositoryManager repository)
    {
        _repository = repository;
    }

    public IEnumerable<Document> GetAllDocumentsForTrip(long userId, long tripId) =>
        _repository.Document.FindAllDocumentsForTrip(userId, tripId);

    public IEnumerable<Document> GetAllDocumentsForActivity(long userId, long activityId) =>
        _repository.Document.FindAllDocumentsForActivity(userId, activityId);

    public Document? GetDocument(long documentId) => _repository.Document.FindDocument(documentId);

    public async Task<Document> CreateDocument(long userId, DocumentCreateDto document)
    {
        Document documentToCreate =
            new()
            {
                Name = document.Name,
                FilePath = document.FilePath,
                Type = document.Type,
                UserId = userId,
            };
        _repository.Document.CreateDocument(documentToCreate);
        await _repository.Save();
        var createdDocument = _repository.Document.FindDocument(documentToCreate.Id);
        return createdDocument;
    }

    public async Task<bool> DeleteDocument(long documentId)
    {
        Document document = _repository.Document.FindDocument(documentId);

        await _repository.Storage.DeleteFile(
            BucketNames.TouristarFirebase,
            $"touristar-activity-documents/{document.FileName}"
        );

        _repository.Document.DeleteDocument(document);
        await _repository.Save();
        return true;
    }
}
