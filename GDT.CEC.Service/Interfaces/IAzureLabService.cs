using GDT.CEC.Repository.Models.AzureLabs;
using GDT.CEC.Repository.Models.PageTemplate;
using GDT.CEC.Service.DTOs;

namespace GDT.CEC.Service.Interfaces
{
    public interface IAzureLabService
    {
        Task<LabDTO> GetLab(string id);
        Task<List<AzureLab>> GetLabs(bool getActiveOnly, bool detailed);
        Task<List<AzureLab>> GetLabsForUser(string userOid);
        Task<bool> CreateLabAsync(CreateLabDTO createLabDTO);
        Task<bool> UpdateLabActiveAsync(string azureid, bool isActive);
        Task<bool> UpdateLabAsync(CreateLabDTO createLabDTO);
    }
}