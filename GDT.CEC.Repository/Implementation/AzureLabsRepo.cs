using GDT.CEC.Repository.Models.AzureLabs;
using GDT.CEC.Repository.Models.PageTemplate;
using MongoDB.Bson;

namespace GDT.CEC.Repository.Implementation
{
    public class AzureLabsRepo : IAzureLabsRepo
    {
        private readonly IMongoCollection<AzureLab> _collectionLab;
        private IMongoCollection<User> _collectionUser;

        public IMongoDBManager _DbContext { get; }

        public AzureLabsRepo(IMongoDBManager dbContext)
        {
            _collectionLab = dbContext.GetCollection<AzureLab>(AppConstants.COLLECTION_LABS);
            _DbContext = dbContext;
        }
        public async Task<List<AzureLab>> GetLabs(bool getActiveOnly, bool detailed)
        {
            var labs = await _collectionLab.Find(_ => true).ToListAsync();
            if(getActiveOnly)
            {
                labs = labs.Where(m => m.IsActive == true).ToList();
            }
            if (detailed)
            {
                return labs;
            }
            else
            {
                return labs.ToList().Select(l => new AzureLab { AzureLabID = l.AzureLabID, Name = l.Name,IsActive=l.IsActive }).ToList();
            }
        }
        public async Task<List<AzureLab>> GetLabsForUser(string userOid)
        {
            _collectionUser = _DbContext.GetCollection<User>(AppConstants.COLLECTION_USER);
            var user = _collectionUser.Find(p => p.AzureObjectID == userOid).SingleOrDefault();
            var labIds = user.Labs.Select(m=>m.AzureLabID).ToList();
            var filter = Builders<AzureLab>.Filter.In(x => x.AzureLabID, labIds);
            var matchingDocuments =await  _collectionLab.Find(filter).ToListAsync();
            return matchingDocuments;
        }
        public async Task<AzureLab> GetLab(string id)
        {
           return  await _collectionLab.Find(p => p.AzureLabID == id).FirstOrDefaultAsync();
        }
        public async Task<bool> CreateLabAsync(AzureLab newLab)
        {           
            try
            {
                await _collectionLab.InsertOneAsync(newLab);
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> UpdateLabActiveAsync(string azureid, bool isActive)
        {
            var filter = Builders<AzureLab>.Filter.Eq(u => u.AzureLabID, azureid);
            var update = Builders<AzureLab>.Update.Set(u => u.IsActive, isActive);
            var result = await _collectionLab.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateLabAsync(AzureLab azureLab)
        {
            var filter = Builders<AzureLab>.Filter.Eq("AzureLabID", azureLab.AzureLabID);
            var update = Builders<AzureLab>.Update
                .Set("Name", azureLab.Name)
                .Set("HostPoolName", azureLab.HostPoolName)
                .Set("Description", azureLab.Description)
                .Set("LaunchLink", azureLab.LaunchLink)
                .Set("ButtonText", azureLab.ButtonText)
                .Set("ButtonText2", azureLab.ButtonText2)
                .Set("Image", azureLab.Image)
                .Set("TabsData", azureLab.TabsData)
                .Set("Categories", azureLab.Categories)
                .Set("IsActive", azureLab.IsActive);
            var result = await _collectionLab.UpdateOneAsync(filter, update);
            return result.MatchedCount > 0;
        }
    }
}
