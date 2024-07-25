using Azure.Identity;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sample.Integration.AzureFunctionApp.Adapter;
using Sample.Integration.AzureFunctionApp.Configs;
using Sample.Integration.AzureFunctionApp.Interfaces;
using Sample.Integration.AzureFunctionApp.Repository;
using Sample.Integration.AzureFunctionApp.Services;
using Serilog;
using System;
using System.IO;

[assembly: FunctionsStartup(typeof(Sample.Integration.AzureFunctionApp.Startup))]
namespace Sample.Integration.AzureFunctionApp
{
    public class Startup : FunctionsStartup
    {
        AppSettings settings;
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var allowSpecificOrigins = "allowSubdomainPolicy";
            builder.Services.AddSingleton<IAppSettings>(settings);
            builder.Services.AddSingleton<ITokenCache, TokenCache>();
            builder.Services.AddScoped<IConnectionService, ConnectionService>();
            builder.Services.AddScoped<IOpportunityRepository, OpportunityRepository>();
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<IDataverseService>(service => new DataverseService(
                service.GetRequiredService<IOpportunityRepository>(),
                service.GetRequiredService<IAccountRepository>()));
            builder.Services.AddHttpClient<IConnectionService, ConnectionService>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(5);
            });
            builder.Services.AddMemoryCache();
            var logger = new LoggerConfiguration().ReadFrom
                            .Configuration(AppSettings.Configs)
                            .Enrich.FromLogContext()
                            .CreateLogger();
            builder.Services.AddLogging(lg => lg.AddSerilog(logger));
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: allowSpecificOrigins,
                    policy =>
                    {
                        policy.WithOrigins("https://*.crm5.dynamics.com")
                            .SetIsOriginAllowedToAllowWildcardSubdomains();
                    });
            });
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            FunctionsHostBuilderContext context = builder.GetContext();
            var configuration = builder.ConfigurationBuilder.AddJsonFile(Path.Combine(context.ApplicationRootPath, "appsettings.json"), false, true)
                .AddEnvironmentVariables();

            string userAssignedClientId = "";
            var keyvaultName = Environment.GetEnvironmentVariable("KeyVaultName");
            var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = userAssignedClientId, TenantId = "", ExcludeEnvironmentCredential = true });

            configuration.AddAzureKeyVault(new Uri($"https://{keyvaultName}.vault.azure.net/"), credential);
            var configs = configuration.Build();
            settings = new AppSettings();
            configs.GetSection("AppSettings").Bind(settings);
            AppSettings.Configs = configs;
        }
    }
}
