using Microsoft.AspNetCore.Mvc;
using GDT.CEC.Repository.Models.Configurations;
using Microsoft.Extensions.Options;

namespace GDT.CEC.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly AzureADConfig _azureADConfig;
        private readonly MongoDBConfig _mongoConfig;

        public TestController(IOptions<AzureADConfig> azureADConfig, IOptions<MongoDBConfig> mongoConfig)
        {
            _azureADConfig = azureADConfig.Value;
            _mongoConfig = mongoConfig.Value;
        }

        [HttpGet("keyvault-status")]
        public IActionResult GetKeyVaultStatus()
        {
            return Ok(new
            {
                KeyVaultUsed = !string.IsNullOrEmpty(_azureADConfig.AzureADClientID),
                ClientIdLoaded = !string.IsNullOrEmpty(_azureADConfig.AzureADClientID),
                TenantIdLoaded = !string.IsNullOrEmpty(_azureADConfig.AzureADTenentID),
                SecretLoaded = !string.IsNullOrEmpty(_azureADConfig.AzureADClientSecret),
                ConnectionStringLoaded = !string.IsNullOrEmpty(_mongoConfig.ConnectionString),
                Message = "Key Vault integration test"
            });
        }
    }
}