using GDT.CEC.Repository.Models.AzureLabs;
using GDT.CEC.Repository.Models.PageTemplate;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDT.CEC.Repository.Implementation
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IMongoCollection<Category> _collectionCategory;
        private readonly IMongoDBManager _dbContext;
        private  IMongoCollection<AzureLab> _collectionLabs;

        public CategoryRepository(IMongoDBManager dbContext)
        {
            _dbContext = dbContext;
            _collectionCategory = dbContext.GetCollection<Category>(AppConstants.COLLECTION_CATEGORY);
        }

        public async Task<List<Category>> GetCategories(string type, string categoryName)
        {
            var categories = await _collectionCategory.Find(m => m.Key == categoryName || categoryName == "all").ToListAsync();
            if (type == "detailed")
            {
                _collectionLabs = _dbContext.GetCollection<AzureLab>(AppConstants.COLLECTION_LABS);
                foreach (Category ct in categories)
                {
                    ct.ID = ct.ID.Replace("ObjectId(\"", "").Replace("\")", "");
                    if (ct.TabsData != null)
                    {
                        foreach (var tab in ct.TabsData)
                        {
                            if (tab.Key.ToLower() == "laas")
                            {
                                tab.Labs = new MasterLabs();
                                tab.Labs.Items = _collectionLabs.Find(m => m.IsActive == true).ToList();
                            }
                        }
                    }
                }
                return categories;
            }
            else
            {
                return categories.ToList().Select(l => new Category
                {
                    ID = l.ID.Replace("ObjectId(\"", "").Replace("\")", ""),
                    Header = l.Header,
                    Key=l.Key,
                    Description = l.Description
                }).ToList();
            }
        }

        public async Task<bool> CreateLabCategoryAsync(Category category)
        {             
            try
            {
                await _collectionCategory.InsertOneAsync(category);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            var filter = Builders<Category>.Filter.Eq(c => c.Key, category.Key);

            var update = Builders<Category>.Update
                      .Set(x => x.Header, category.Header)
                      .Set(x => x.TabsData, category.TabsData)
                      .Set(x => x.Description, category.Description);
            await _collectionCategory.UpdateOneAsync(filter, update);

            var result = await _collectionCategory.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }
        public async Task<bool> UpdateCategoryTabsAsync(Category category)
        {
            var filter = Builders<Category>.Filter.Eq(c => c.Key, category.Key);

            var update = Builders<Category>.Update
                      .Set(x => x.TabsData, category.TabsData);
            await _collectionCategory.UpdateOneAsync(filter, update);

            var result = await _collectionCategory.UpdateOneAsync(filter, update);

            return result.MatchedCount > 0;
        }

        public async Task<bool> DeleteCategoryAsync(string key)
        {
            var filter = Builders<Category>.Filter.Eq(c => c.Key, key); 
            var result = await _collectionCategory.DeleteOneAsync(filter);
            return result.DeletedCount > 0; 
        }

    }
}
