using GDT.CEC.Repository.Helpers;
using GDT.CEC.Repository.Models.AzureLabs;
using GDT.CEC.Repository.Models.Configurations;
using GDT.CEC.Repository.Models.HttpClient;
using GDT.CEC.Repository.Models.Response;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDT.CEC.Service.Implementation
{
    public class AnalyticsSchedulerService : IJob
    {
        private readonly IAnalyticsRepo _analyticsRepo;
        private readonly ILogger<CollectAnalyticsService> _logger;
        private readonly IOptions<AzureADConfig> _azureADConfig;
        private readonly IOptions<AzureAdGetUsersConfig> _azureADGetUsersConfig;
        private readonly IOptions<AzureAdConfigCodinCity> _azureAdConfigCodincity;
        private readonly IOptions<MailConfig> _mailConfig;

        public AnalyticsSchedulerService(IAnalyticsRepo analyticsRepo,ILogger<CollectAnalyticsService> logger, IOptions<AzureADConfig> azureADConfig,
            IOptions<AzureAdGetUsersConfig> azureADGetUsersConfig, IOptions<AzureAdConfigCodinCity> azureAdConfigCodincity, IOptions<MailConfig> mailConfig)
        {
            _logger = logger;
            _azureADConfig = azureADConfig;
            _azureADGetUsersConfig = azureADGetUsersConfig;
            _azureAdConfigCodincity = azureAdConfigCodincity;
            _mailConfig = mailConfig;
            _analyticsRepo = analyticsRepo;
        }


        public async void CollectData()
        {
            try
            {
                DateTime? dateTime = await _analyticsRepo.GetLatestLaunchTime();
                dateTime = dateTime ?? DateTime.Now.AddDays(-7);
                dateTime = dateTime.Value.AddMilliseconds(1);
                AzureAPIManager azureAPIManager = new AzureAPIManager(_azureADConfig.Value, _azureADGetUsersConfig.Value,_azureAdConfigCodincity.Value);
                APIResponse<List<AzureAnalyticsApiRow>> response = await azureAPIManager.GetAnalyticsLogs((DateTime)dateTime);

                if (response.StatusCode == 1)
                {
                    List<LabAccessLog> lst = new List<LabAccessLog>();
                    foreach (AzureAnalyticsApiRow row in response.Data)
                    {
                        LabAccessLog labAccessLog = new LabAccessLog
                        {
                            ClosedAt = row.EndTime,
                            Duration = row.Duration.ToString(),
                            LaunchedAt = row.StartTime,
                            LabName = row.HostPoolName,
                            UserName = row.UserName
                        };
                        lst.Add(labAccessLog);
                    }
                    if (lst.Count > 0)
                    {
                        await _analyticsRepo.CreateAsync(lst);
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,ex.Message);
            }
        }
        public Task Execute(IJobExecutionContext context)
        {
            CollectData();
           return Task.CompletedTask;
        }

    }
}
