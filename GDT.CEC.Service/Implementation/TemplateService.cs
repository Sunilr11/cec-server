using GDT.CEC.Repository.Models.PageTemplate;
using GDT.CEC.Service.DTOs;
using MongoDB.Driver;

namespace GDT.CEC.Service.Implementation
{
    public class TemplateService : ITemplateService
    {
        private readonly ITemplateRepo _templateRepo;
        private readonly ICategoryRepository _categoryRepository;

        public TemplateService(ITemplateRepo templateRepo, ICategoryRepository categoryRepository)
        {
            _templateRepo = templateRepo;
            _categoryRepository = categoryRepository;
        }

        public Task<List<Category>> GetCategories(string type, string categoryName)
        {
            return _categoryRepository.GetCategories(type, categoryName);
        }

        public async Task<bool> CreateLabCategoryAsync(CategoryDTO categoryDto)
        {
            var category = new Category
            {
                Key = categoryDto.Key,
                Header = categoryDto.Header,
                Description = categoryDto.Description,
                TabsData = categoryDto.TabsData
            };
            return await _categoryRepository.CreateLabCategoryAsync(category);
        }

        public async Task<bool> UpdateCategoryAsync(CategoryDTO categoryDto)
        {
            var category = new Category
            {
                Key = categoryDto.Key,
                Header = categoryDto.Header,
                Description = categoryDto.Description,
                TabsData = categoryDto.TabsData
            };
            return await _categoryRepository.UpdateCategoryAsync(category);
        }
        public async Task<bool> UpdateCategoryTabsAsync(CategoryTabDTO categoryDto)
        {
            var category = new Category
            {
                Key = categoryDto.Key,
                TabsData = categoryDto.TabsData
            };
            return await _categoryRepository.UpdateCategoryTabsAsync(category);
        }
        public async Task<bool> DeleteCategoryAsync(string key)
        {
            return await _categoryRepository.DeleteCategoryAsync(key); 
        }

        public Task<Menu> GetMenu()
        {
            return _templateRepo.GetMenu();
        }

        public Task<HomeTemplate> GetHomeTemplate()
        {
            return _templateRepo.GetHomeTemplate();
        }
        public Task<MasterTemplate> GetMasterTemplate(string name)
        {
            return _templateRepo.GetMasterTemplate(name);
        }
        public Task<AboutTemplate> GetAboutTemplate()
        {
            return _templateRepo.GetAboutTemplate();
        }

        public async Task<IEnumerable<MenuDetails>> GetMenuDetailsByRoleAsync(int roleId)
        {
            var allMenus = await _templateRepo.GetAllMenuDetailsAsync();
            var allRoles = await _templateRepo.GetAllRoleAsync();

            if (allMenus == null || !allMenus.Any())
            {
                throw new Exception("MenuDetails collection is empty or not fetched properly.");
            }

            if (allRoles == null || !allRoles.Any())
            {
                throw new Exception("Role collection is empty or not fetched properly.");
            }

            if (!allRoles.Any(role => role.RoleId == roleId))
            {
                throw new KeyNotFoundException($"Role with ID '{roleId}' not found.");
            }

            var filteredMenus = allMenus.Where(menu => menu.RoleIds != null && menu.RoleIds.Contains(roleId)).ToList();

            var menuTree = BuildMenuTree(filteredMenus);

            return menuTree;
        }

        private List<MenuDetails> BuildMenuTree(List<MenuDetails> menus)
        {
            var menuMap = menus.ToDictionary(menu => menu.MenuId, menu => new MenuDetails
            {
                MenuId = menu.MenuId,
                Name = menu.Name,
                IconPath = menu.IconPath,
                ParentId = menu.ParentId,
                RoleIds = menu.RoleIds,
                Order = menu.Order,
                createdAt = menu.createdAt,
                updatedAt = menu.updatedAt,
                IsActive = menu.IsActive,
                Link = menu.Link,
                Key = menu.Key,
                Children = new List<MenuDetails>()
            });

            var menuTree = new List<MenuDetails>();

            foreach (var menu in menus)
            {
                if (!string.IsNullOrEmpty(menu.ParentId) && menuMap.ContainsKey(menu.ParentId))
                {
                    menuMap[menu.ParentId].Children.Add(menuMap[menu.MenuId]);
                }
                else
                {
                    menuTree.Add(menuMap[menu.MenuId]);
                }
            }

            return menuTree.OrderBy(m => m.Order).ToList();
        }

    }
}