using GDT.CEC.Repository.Models.PageTemplate;
using GDT.CEC.Service.DTOs;

namespace GDT.CEC.Service.Interfaces
{
    public interface ITemplateService
    {
        Task<HomeTemplate> GetHomeTemplate();
        Task<Menu> GetMenu();
        Task<MasterTemplate> GetMasterTemplate(string name);
        Task<AboutTemplate> GetAboutTemplate();
        Task<List<Category>> GetCategories(string type, string categoryName);
        Task<bool> DeleteCategoryAsync(string key);
        Task<bool> UpdateCategoryAsync(CategoryDTO categoryDto);
        Task<bool> CreateLabCategoryAsync(CategoryDTO categoryDto);
        Task<bool> UpdateCategoryTabsAsync(CategoryTabDTO categoryDto);
        Task<IEnumerable<MenuDetails>> GetMenuDetailsByRoleAsync(int roleId);
    }
}