using GDT.CEC.Repository.Models.PageTemplate;

namespace GDT.CEC.Repository.Interface
{
    public interface ITemplateRepo
    {
        Task<HomeTemplate> GetHomeTemplate();
        public  Task<Menu> GetMenu();
        Task<MasterTemplate> GetMasterTemplate(string name);
        Task<AboutTemplate> GetAboutTemplate();
        Task<bool> RoleExistsAsync(int roleId);
        Task<IEnumerable<Role>> GetAllRoleAsync();
        Task<IEnumerable<MenuDetails>> GetAllMenuDetailsAsync();
    }
}