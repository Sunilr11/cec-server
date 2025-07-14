using GDT.CEC.Repository.Models.AzureLabs;
using GDT.CEC.Service.DTOs;

namespace GDT.CEC.Service.Implementation
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IAnalyticsRepo _analyticsRepo;

        public AnalyticsService(IAnalyticsRepo analyticsRepo)
        {
            _analyticsRepo = analyticsRepo;
        }

        public async Task<bool> CreateAsync(LabAccessLogDTO logDto)
        {
            try
            {
                bool result = false;
                LabAccessLog labAccessLog = new LabAccessLog
                {
                    LabName = logDto.LabName,
                    UserName = logDto.UserName,
                    LaunchedAt = logDto.LaunchedAt,
                    ClosedAt = logDto.ClosedAt
                };
                await _analyticsRepo.CreateAsync(labAccessLog);

                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<LabAcccessLogPagingModel> GetLabAccessLogsAsync(int pageno, int pagesize)
        {
            var logs = await _analyticsRepo.GetLabAccessLogDTOsAsync(pageno, pagesize);
            List<LabAccessLogDTO> labAccessLogs = new List<LabAccessLogDTO>();
            foreach (LabAccessLogRepoDTO log in logs.Logs)
            {
                labAccessLogs.Add(new LabAccessLogDTO
                {
                    LabName = log.LabName,
                    UserFullName = log.FirstName + " " + log.LastName,
                    LaunchedAt = log.LaunchedAt,
                    ID = log.ID,
                    UserName = log.UserName,
                    UserType = log.UserType,
                    Duration = log.Duration
                });
            }
            LabAcccessLogPagingModel pagingModel = new LabAcccessLogPagingModel { Logs = labAccessLogs, PageSize = pagesize, CurrentPage = pageno, TotalCount = logs.TotalCount };
            return pagingModel;
        }

        public async Task<LabAccessLogCountDTO> GetLabAccessLogCounts(string filter)
        {
            var analytics = await _analyticsRepo.GetLabAccessCount(filter);

            return analytics;
        }

        public async Task<LabAccessDetails> GetLabAccessDetailByLab(string labHostname)
        {
            var det=await _analyticsRepo.GetLabAccessDetailByLab(labHostname);
            LabAccessDetails details = new LabAccessDetails { TotalSessions = det.Item1, TotalDuration = det.Item2 };
            return details;
        }

    }
}
