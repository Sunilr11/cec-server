using Microsoft.Extensions.Options;
using MongoDB.Bson;
using GDT.CEC.Repository.Models.Configurations;


namespace GDT.CEC.Repository.Core
{
    public class MongoDBManager : IMongoDBManager
    {
        private readonly IMongoDatabase _database;
        private readonly MongoDBConfig _dbConfig;

        public MongoDBManager(IOptions<MongoDBConfig> dbConfig)
        {
            _dbConfig = dbConfig.Value;
            var client = new MongoClient(_dbConfig.ConnectionString);
            _database = client.GetDatabase(_dbConfig.DatabaseName);
        }
        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }

        public async Task<bool> CollectionExistsAsync(string collectionName)
        {
            var collectionNames = new List<string>();

            await (await _database.ListCollectionNamesAsync())
                .ForEachAsync(name =>
                {
                    collectionNames.Add(name);
                });

            return collectionNames.Contains(collectionName);
        }

        public async Task<bool> IsTTLIndexExistsAsync(string collectionName, string ttlFieldName)
        {
            var collection = _database.GetCollection<BsonDocument>(collectionName);
            var indexes = await collection.Indexes.ListAsync();

            return indexes.ToEnumerable().Any(index => index.Contains($"{ttlFieldName}_1"));
        }

        public async Task CreateTTLIndexAsync<T>(string collectionName, string ttlFieldName) where T : class
        {
            var collection = _database.GetCollection<T>(collectionName);

            var ttlIndexDefinition = new CreateIndexModel<T>(
                Builders<T>.IndexKeys.Ascending(ttlFieldName),
                new CreateIndexOptions
                {
                    ExpireAfter = TimeSpan.Zero 
                });

            await collection.Indexes.CreateOneAsync(ttlIndexDefinition);
        }
    }
}
