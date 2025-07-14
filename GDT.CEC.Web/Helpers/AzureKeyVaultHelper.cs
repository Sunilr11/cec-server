using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
 
namespace GDT.CEC.Web.Helpers
{
    public class AzureKeyVaultHelper
    {
        private readonly ILogger<AzureKeyVaultHelper> _logger;
        private readonly string _keyVaultUrl;
 
        public AzureKeyVaultHelper(ILogger<AzureKeyVaultHelper> logger, IConfiguration configuration)
        {
            _logger = logger;
            _keyVaultUrl = configuration["AZURE_KEY_VAULT_URL"];
 
            if (string.IsNullOrEmpty(_keyVaultUrl))
            {
                _logger.LogError("[AzureKeyVaultHelper] KeyVault URL not found in configuration.");
                throw new ArgumentNullException(nameof(_keyVaultUrl), "KeyVault URL is required.");
            }
        }
 
        public async Task<string> GetSecretAsync(string secretName)
        {
            try
            {
                var credential = new DefaultAzureCredential();
                var client = new SecretClient(new Uri(_keyVaultUrl), credential);
                KeyVaultSecret secret = await client.GetSecretAsync(secretName);
                return secret.Value;
            }
            catch (Azure.RequestFailedException ex)
            {
                _logger.LogError(ex, $"[{nameof(GetSecretAsync)}] Azure Request Error ({ex.Status}): {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{nameof(GetSecretAsync)}] Unexpected error: {ex.Message}");
            }
 
            return null;
        }
    }
}