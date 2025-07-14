using GDT.CEC.Repository.Models.AzureLabs;
using GDT.CEC.Service.DTOs;

namespace GDT.CEC.Service.Interfaces
{
    public interface IAnalyticsService
    {
        Task<bool> CreateAsync(LabAccessLogDTO logDto);
        Task<LabAccessDetails> GetLabAccessDetailByLab(string labHostname);
        Task<LabAccessLogCountDTO> GetLabAccessLogCounts(string filter);
        Task<LabAcccessLogPagingModel> GetLabAccessLogsAsync(int pageno, int pagesize);
    }
}