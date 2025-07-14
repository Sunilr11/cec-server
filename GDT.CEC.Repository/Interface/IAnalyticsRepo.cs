using GDT.CEC.Repository.Models.AzureLabs;
using MongoDB.Bson;

namespace GDT.CEC.Repository.Interface
{
    public interface IAnalyticsRepo
    {
        Task CreateAsync(LabAccessLog model);
        Task CreateAsync(List<LabAccessLog> model);
        Task<LabAccessLogCountDTO> GetLabAccessCount(string filter = "all");
        Task<(int, TimeSpan)> GetLabAccessDetailByLab(string labHostname);
        Task<LabAccessLogRepoPagingDTO> GetLabAccessLogDTOsAsync(int pageNo, int pageSize);
        Task<DateTime?> GetLatestLaunchTime();
    }
}