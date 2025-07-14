using GDT.CEC.Repository.Models.AzureLabs;
using GDT.CEC.Repository.Models.PageTemplate;

namespace GDT.CEC.Repository.Interface
{
    public interface IAzureLabsRepo
    {
        Task<AzureLab> GetLab(string id);
        Task<List<AzureLab>> GetLabs(bool getActiveOnly, bool detailed);
        Task<List<AzureLab>> GetLabsForUser(string userOid);
        Task<bool> CreateLabAsync(AzureLab newLab);

        Task<bool> UpdateLabActiveAsync(string azureid, bool isActive);

        Task<bool> UpdateLabAsync(AzureLab azureLab);
    }
}