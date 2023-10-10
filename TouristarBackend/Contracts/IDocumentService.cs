using TouristarModels.Models;

namespace TouristarBackend.Contracts;

public interface IDocumentService
{
    IEnumerable<Document> GetAllDocumentsForTrip(long userId, long tripId);
    IEnumerable<Document> GetAllDocumentsForActivity(long userId, long activityId);
    Document? GetDocument(long documentId);
    Task<Document> CreateDocument(long userId, DocumentCreateDto trip);
    Task<bool> DeleteDocument(long documentId);
}