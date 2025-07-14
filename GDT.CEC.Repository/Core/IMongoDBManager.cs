
namespace GDT.CEC.Repository.Core
{
    public interface IMongoDBManager
    {
        IMongoCollection<T> GetCollection<T>(string collectionName);
        Task<bool> CollectionExistsAsync(string collectionName);
        Task<bool> IsTTLIndexExistsAsync(string collectionName, string ttlFieldName);
        Task CreateTTLIndexAsync<T>(string collectionName, string ttlFieldName) where T : class;
    }
}
