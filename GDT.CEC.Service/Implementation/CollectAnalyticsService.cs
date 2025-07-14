using Amazon.Runtime.Internal.Util;
using GDT.CEC.Repository.Helpers;
using GDT.CEC.Repository.Models.Configurations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDT.CEC.Service.Implementation
{
    public  class CollectAnalyticsService
    {
        private readonly ILogger<CollectAnalyticsService> _logger;
        private readonly IOptions<AzureADConfig> _azureADConfig;
        private readonly IOptions<AzureAdGetUsersConfig> _azureADGetUsersConfig;
        private readonly IOptions<AzureAdConfigCodinCity> _azureAdConfigCodincity;

        public CollectAnalyticsService(ILogger<CollectAnalyticsService> logger, IOptions<AzureADConfig> azureADConfig,
            IOptions<AzureAdGetUsersConfig> azureADGetUsersConfig, IOptions<AzureAdConfigCodinCity> azureAdConfigCodincity)
        {
            _logger = logger;
            _azureADConfig = azureADConfig;
            _azureADGetUsersConfig = azureADGetUsersConfig;
            _azureAdConfigCodincity = azureAdConfigCodincity;
        }

        public void CollectData()
        {
            AzureAPIManager azureAPIManager = new AzureAPIManager(_azureADConfig.Value, _azureADGetUsersConfig.Value,_azureAdConfigCodincity.Value);
        }
    }
}
