using System;
using System.Linq;
using System.Threading.Tasks;
using Acme.JsonTestConsoleApp.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Volo.Abp;
using Volo.Abp.Data;

namespace Acme.JsonTestConsoleApp;

public class Program
{
    public async static Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Async(c => c.File("Logs/logs.txt"))
            .WriteTo.Async(c => c.Console())
            .CreateLogger();

        try
        {
            Log.Information("Starting console host.");

            var builder = Host.CreateApplicationBuilder(args);

            builder.Configuration.AddAppSettingsSecretsJson();
            builder.Logging.ClearProviders().AddSerilog();

            builder.ConfigureContainer(builder.Services.AddAutofacServiceProviderFactory());

            if (IsMigrateDatabase(args))
            {
                Log.Information("运行迁徙文件！！！！！！！！！！！！");
                builder.Services.AddDataMigrationEnvironment();
            }

            builder.Services.AddHostedService<JsonTestConsoleAppHostedService>();

            await builder.Services.AddApplicationAsync<JsonTestConsoleAppModule>();

            var host = builder.Build();

            await host.InitializeAsync();

            if (IsMigrateDatabase(args))
            {
                await host.Services.GetRequiredService<ToJsonTestDbMigrationService>().MigrateAsync();
                return 0;
            }


            await host.RunAsync();

            return 0;
        }
        catch (Exception ex)
        {
            if (ex is HostAbortedException)
            {
                throw;
            }

            Log.Fatal(ex, "Host terminated unexpectedly!");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static bool IsMigrateDatabase(string[] args)
    {
        return args.Any(x => x.Contains("--migrate-database", StringComparison.OrdinalIgnoreCase));
    }
}
