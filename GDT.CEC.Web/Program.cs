using GDT.CEC.Repository.Core;
using GDT.CEC.Repository.Implementation;
using GDT.CEC.Repository.Interface;
using GDT.CEC.Repository.Models.Configurations;
using GDT.CEC.Service.Implementation;
using GDT.CEC.Web.Middleware;
using GDT.CEC.Web.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using Quartz;
using Serilog;
using System.Text.Json;




var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

// Register AzureKeyVaultHelper
builder.Services.AddSingleton<AzureKeyVaultHelper>();


builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddMicrosoftIdentityWebApi(builder.Configuration, "AzureADLogin");


string AllowedOrigins = Configuration.GetValue<string>(ConfigurationConstants.APP_ALLOWED_ORIGINS);

if (!string.IsNullOrEmpty(AllowedOrigins))
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: "AllowOrigin",
            builder =>
            {
                builder.WithOrigins(AllowedOrigins.Split(';'))
                                    .AllowAnyHeader()
                                    .AllowAnyMethod();
            });
    });
}

string instKey = builder.Configuration.GetSection("ApplicationInsights")["InstrumentationKey"];

var loggerSerilog = new LoggerConfiguration()
.ReadFrom.Configuration(builder.Configuration)
.Enrich.FromLogContext()
.WriteTo.ApplicationInsights(instKey, TelemetryConverter.Traces)
.CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(loggerSerilog);



builder.Services.Configure<ApiBehaviorOptions>(x =>
{
    x.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = Configuration.GetValue<string>(ConfigurationConstants.REDISCACHE_CONNECTION);
});

builder.Services.AddMemoryCache();


builder.Services.AddControllers();
builder.Services.AddSingleton<IMongoDBManager, MongoDBManager>();
builder.Services.AddTransient<IUsersService, UsersService>();
builder.Services.AddTransient<IUsersRepo, UsersRepo>();
builder.Services.AddTransient<ITemplateService, TemplateService>();
builder.Services.AddTransient<ITemplateRepo, TemplateRepo>();
builder.Services.AddTransient<IAzureLabService, AzureLabService>();
builder.Services.AddTransient<IAzureLabsRepo, AzureLabsRepo>();
builder.Services.AddTransient<IAnalyticsRepo, AnalyticsRepo>();
builder.Services.AddTransient<IAnalyticsService, AnalyticsService>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<IImageService, ImageService>();


builder.Services.AddTransient<AnalyticsSchedulerService>();


builder.Services.Configure<AppConfig>(Configuration.GetSection(ConfigurationConstants.APP_CONFIG));
builder.Services.Configure<AzureStorageConfig>(Configuration.GetSection("AzureStorageConfig"));

builder.Services.Configure<CacheConfig>(x =>
{
    x.CacheCheck = true;
    x.AllUser_Cache_ExpiryTime = Configuration.GetValue<int>(ConfigurationConstants.ALLUSER_CACHE_EXPIRYTIME);
    x.CSP_Cache_ExpiryTime = Configuration.GetValue<int>(ConfigurationConstants.CSP_CACHE_EXPIRYTIME);
    x.OTP_ExpiryTime = Configuration.GetValue<int>(ConfigurationConstants.OTP_Expiry);
});


builder.Services.Configure<MailConfig>(x =>
{
    x.SMTPServer = Configuration.GetValue<string>(ConfigurationConstants.MAIL_SMTPSERVER);
    x.SMTPPort = Configuration.GetValue<int>(ConfigurationConstants.MAIL_PORT);
    x.From = Configuration.GetValue<string>(ConfigurationConstants.MAIL_FROM);
    x.SMTPUsername = Configuration.GetValue<string>(ConfigurationConstants.MAIL_USERNAME);
    x.SMTPPassword = Configuration.GetValue<string>(ConfigurationConstants.MAIL_PASSWORD);
    x.EnableSSL = Configuration.GetValue<bool>(ConfigurationConstants.MAIL_ENABLESSL);
    x.EmailTemplateImagePath = Configuration.GetValue<string>(ConfigurationConstants.MAIL_IMAGEPATH);
    x.DisplayName = Configuration.GetValue<string>(ConfigurationConstants.MAIL_DISPLAYNAME);
});

builder.Services.AddQuartz(q =>
{
    q.ScheduleJob<AnalyticsSchedulerService>(trigger => trigger
        .WithIdentity("AnalyticsJobTrigger")
        .StartAt(DateTime.UtcNow.AddSeconds(30))
        .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(5)
            .RepeatForever())
    );
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);



// Configure Azure AD settings with Key Vault secrets
var keyVaultHelper = builder.Services.BuildServiceProvider().GetService<AzureKeyVaultHelper>();
if (keyVaultHelper != null)
{
    var clientId = await keyVaultHelper.GetSecretAsync("GDT-CEC-ClientID");
    var tenantId = await keyVaultHelper.GetSecretAsync("GDT-CEC-TenantID");
    var clientSecret = await keyVaultHelper.GetSecretAsync("GDT-CEC-ClientSecret");
    var connectionString = await keyVaultHelper.GetSecretAsync("GDT-CEC-ConnectionString");
    
    // Log Key Vault usage (without exposing secrets)
    Console.WriteLine($"[KEY VAULT] ClientID loaded: {!string.IsNullOrEmpty(clientId)}");
    Console.WriteLine($"[KEY VAULT] TenantID loaded: {!string.IsNullOrEmpty(tenantId)}");
    Console.WriteLine($"[KEY VAULT] ClientSecret loaded: {!string.IsNullOrEmpty(clientSecret)}");
    Console.WriteLine($"[KEY VAULT] ConnectionString loaded: {!string.IsNullOrEmpty(connectionString)}");

    builder.Services.Configure<AzureADConfig>(config =>
    {
        Configuration.GetSection(ConfigurationConstants.AZUREAD_CONFIG).Bind(config);
        config.AzureADClientID = clientId;
        config.AzureADTenentID = tenantId;
        config.AzureADClientSecret = clientSecret;
    });

    builder.Services.Configure<AzureAdGetUsersConfig>(config =>
    {
        Configuration.GetSection("AzureAdGetUsersConfig").Bind(config);
        config.AzureADClientID = clientId;
        config.AzureADTenentID = tenantId;
        config.AzureADClientSecret = clientSecret;
    });

    builder.Services.Configure<AzureAdConfigCodinCity>(config =>
    {
        Configuration.GetSection("AzureAdConfigCodinCity").Bind(config);
        config.AzureADClientID = clientId;
        config.AzureADTenentID = tenantId;
        config.AzureADClientSecret = clientSecret;
    });

    // Configure AzureADLogin with Key Vault secrets
    Configuration["AzureADLogin:ClientId"] = clientId;
    Configuration["AzureADLogin:TenantId"] = tenantId;

    // Configure MongoDB with Key Vault connection string
    builder.Services.Configure<MongoDBConfig>(config =>
    {
        Configuration.GetSection(ConfigurationConstants.MONGO_CONFIG).Bind(config);
        config.ConnectionString = connectionString;
    });
}
else
{
    builder.Services.Configure<AzureADConfig>(Configuration.GetSection(ConfigurationConstants.AZUREAD_CONFIG));
    builder.Services.Configure<AzureAdGetUsersConfig>(Configuration.GetSection("AzureAdGetUsersConfig"));
    builder.Services.Configure<AzureAdConfigCodinCity>(Configuration.GetSection("AzureAdConfigCodinCity"));
    
    // Fallback MongoDB configuration without Key Vault
    builder.Services.Configure<MongoDBConfig>(Configuration.GetSection(ConfigurationConstants.MONGO_CONFIG));
}


builder.Services.AddSwaggerGen(swagger =>
{  
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "GDT.CEC-WebAPI",
        Description = "GDT.CEC-WebAPI Release 1"
    });
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}
                    }
                    });

    System.IO.Directory.EnumerateFiles(AppContext.BaseDirectory, "GDT.CEC*.xml").ToList().ForEach(x =>
    {
        swagger.IncludeXmlComments(x);
    });
});


var app = builder.Build();

var Env = app.Environment;


if (app.Environment.EnvironmentName.Equals("dev"))
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Test API V1");
    });
}

app.UseCors(builder =>
{
    builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
    .WithExposedHeaders("Content-Disposition");
});

app.UseStaticFiles();
app.UseWebSockets();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseMiddleware<AuthenticationMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

