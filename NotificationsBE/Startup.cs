using NotificationsBE;
using NotificationsBE.DB;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using Azure.Identity;
using System.Reflection;
using Azure.Security.KeyVault.Secrets;
using System.Data.Common;

[assembly: FunctionsStartup(typeof(Startup))]
namespace NotificationsBE
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddSingleton<IConfiguration>(config);

            var client = new SecretClient(vaultUri: new Uri(config["AzureWebJobsSecretStorageKeyVaultUri"]), credential: new DefaultAzureCredential());
            builder.Services.AddSingleton<SecretClient>(client);

            var csb = new DbConnectionStringBuilder { ConnectionString = config.GetConnectionString("CosmosDbConnectionString") };
            var ep = csb.TryGetValue("AccountEndpoint", out object key) ? key.ToString() : config["DbEndpoint"];
            var pk = csb.TryGetValue("AccountKey", out object uri) ? uri.ToString() : config["DbPrimaryKey"];

            builder.Services.AddSingleton((s) => new DocumentClient(new Uri(ep), pk));
            builder.Services.AddTransient<IMessageService, MessageService>();
        }
    }
}
