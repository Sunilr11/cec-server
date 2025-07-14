using GDT.CEC.Repository.Models.PageTemplate;

namespace GDT.CEC.Repository.Interface
{
    public interface ICategoryRepository
    {
        Task<bool> CreateLabCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(string key);
        Task<List<Category>> GetCategories(string type, string categoryName);
        Task<bool> UpdateCategoryAsync(Category category);
        Task<bool> UpdateCategoryTabsAsync(Category category);
    }
}