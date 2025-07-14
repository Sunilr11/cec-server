

namespace GDT.CEC.Repository.Constants
{
    public class EnvironmentConstants
    {
        public const string HOSTING_ENVIRONMENT_DEV = "dev";
        public const string HOSTING_ENVIRONMENT_QA = "uat";
        public const string HOSTING_ENVIRONMENT_PROD = "prod";

        public const string ENV_VAR_AZ_CONFIG_CS = "AZ_CONFIG_CS";
        public const string ENV_VAR_AZ_KEYVAULT_SEC = "AZ_KEY_VAULT_SEC";
        public const string ENV_VAR_AZ_AI_INS_KEY = "AZ_AI_INS_KEY";
    }

    public static class ConfigurationConstants
    {
        public const string APP_CONFIG = "AppConstants";
        public const string AZUREAD_CONFIG = "AzureADConfig";
        public const string MONGO_CONFIG = "MongoConfig";     
        public const string APP_ALLOWED_ORIGINS = "App:AllowedOrigins";
        public const string ALLUSER_CACHE_EXPIRYTIME = "RedisCache:ExpiryTime:AllUsers";
        public const string CSP_CACHE_EXPIRYTIME = "RedisCache:ExpiryTime:CloudProviders";
        public const string REDISCACHE_CONNECTION = "RedisCache:ConnectionString";
        public const string OTP_Expiry = "AppConstants:OTPExpiration";
        public const string COMPANY_NAME = "AppConstants:CompanyName";


        public const string MAIL_CONFIG = "MailConfig";
        public const string MAIL_SMTPSERVER = "MailConfig:SMTPServer";
        public const string MAIL_FROM = "MailConfig:From";
        public const string MAIL_DISPLAYNAME = "MailConfig:DisplayName";
        public const string MAIL_PORT = "MailConfig:SMTPPort";
        public const string MAIL_ENABLESSL = "MailConfig:EnableSSL";
        public const string MAIL_USERNAME = "MailConfig:SMTPUsername";
        public const string MAIL_PASSWORD = "MailConfig:SMTPPassword";
        public const string MAIL_IMAGEPATH = "MailConfig:EmailTemplateImagePath";
    }
}
