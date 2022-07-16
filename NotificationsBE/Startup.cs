using NotificationsBE;
using NotificationsBE.DB;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

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

            var ep = config["DbEndpoint"];
            var pk = config["DbPrimaryKey"];

            builder.Services.AddSingleton((s) => new DocumentClient(new Uri(ep), pk));
            builder.Services.AddTransient<IMessageService, MessageService>();
        }
    }
}
