using GDT.CEC.Repository.Models.AzureLabs;
using GDT.CEC.Repository.Models.PageTemplate;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDT.CEC.Repository.Implementation
{
    public class TemplateRepo : ITemplateRepo
    {
        private readonly IMongoDBManager _dbContext;
        private readonly IMongoDBManager _mongoDBManager;
        private IMongoCollection<Menu> _collectionMenu;
        private IMongoCollection<HomeTemplate> _collectionHomeTemplate;
        private IMongoCollection<MasterTemplate> _collectionTemplate;
        private IMongoCollection<AboutTemplate> _collectionAboutTemplate;
        private IMongoCollection<AzureLab> _collectionLabs;
        private readonly IMongoCollection<MenuDetails> _collectionMenuDetails;
        private readonly IMongoCollection<Role> _collectionrole;
        private readonly ILogger<TemplateRepo> _logger;


        public TemplateRepo(IMongoDBManager dbContext, IMongoDBManager mongoDBManager, ILogger<TemplateRepo> logger)
        {
            _dbContext = dbContext;
            _mongoDBManager = mongoDBManager; _collectionMenuDetails = dbContext.GetCollection<MenuDetails>(AppConstants.COLLECTION_MENUDETAILS);
            _collectionrole = dbContext.GetCollection<Role>(AppConstants.COLLECTION_ROLE);
            _logger = logger;
        }


        public async Task<Menu> GetMenu()
        {
            _collectionMenu = _dbContext.GetCollection<Menu>(AppConstants.COLLECTION_MENU);
            return await _collectionMenu.Find(_ => true).FirstOrDefaultAsync();
        }

        public async Task<bool> RoleExistsAsync(int roleId)
        {
            try
            {
                var filter = Builders<Role>.Filter.Eq(role => role.RoleId, roleId);
                return await _collectionrole.Find(filter).AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking role existence for roleId: {RoleId}", roleId);
                return false;
            }
        }

        public async Task<IEnumerable<MenuDetails>> GetAllMenuDetailsAsync()
        {
            return await _collectionMenuDetails.Find(FilterDefinition<MenuDetails>.Empty).ToListAsync();
        }
        public async Task<IEnumerable<Role>> GetAllRoleAsync()
        {
            return await _collectionrole.Find(FilterDefinition<Role>.Empty).ToListAsync();
        }

        public async Task<HomeTemplate>GetHomeTemplate()
        {
            try
            {
                _collectionHomeTemplate =  _dbContext.GetCollection<HomeTemplate>(AppConstants.COLLECTION_HOME);
                _collectionLabs = _dbContext.GetCollection<AzureLab>(AppConstants.COLLECTION_LABS);
                var template =await _collectionHomeTemplate.Find(_ => true).FirstOrDefaultAsync();
                template.Labs = _collectionLabs.Find(m => m.IsActive == true).ToList();

                return template;
            }
            catch (Exception ex)
            {
                throw;
            }
           
        }
        public async Task<MasterTemplate> GetMasterTemplate(string name)
        {
            try
            {
                switch(name)
                {
                    case "network":
                        _collectionTemplate = _dbContext.GetCollection<MasterTemplate>(AppConstants.COLLECTION_NETWORK);
                        break;
                    case "mobility":
                        _collectionTemplate = _dbContext.GetCollection<MasterTemplate>(AppConstants.COLLECTION_MOBILITY);
                        break;
                    case "digitalworkspace":
                        _collectionTemplate = _dbContext.GetCollection<MasterTemplate>(AppConstants.COLLECTION_DIGITALWORKSPACE);
                        break;
                    case "security":
                        _collectionTemplate = _dbContext.GetCollection<MasterTemplate>(AppConstants.COLLECTION_SECURITY);
                        break;
                    case "cloud":
                        _collectionTemplate = _dbContext.GetCollection<MasterTemplate>(AppConstants.COLLECTION_CLOUD);
                        break;
                }
               var template= await _collectionTemplate.Find(_ => true).FirstOrDefaultAsync();
                _collectionLabs = _dbContext.GetCollection<AzureLab>(AppConstants.COLLECTION_LABS);

                foreach (var tab in template.TabsData)
                {
                    if (tab.Key.ToLower() == "laas")
                    {
                        tab.Labs = new MasterLabs();
                        tab.Labs.Items = _collectionLabs.Find(m => m.IsActive == true).ToList();
                    }
                }
                return template;

            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<AboutTemplate> GetAboutTemplate()
        {
            try
            {
                _collectionAboutTemplate = _dbContext.GetCollection<AboutTemplate>(AppConstants.COLLECTION_ABOUT);
                return await _collectionAboutTemplate.Find(_ => true).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
