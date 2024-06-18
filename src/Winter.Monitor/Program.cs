using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using ServiceSelf;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;

namespace Winter.Monitor;

internal sealed class Program
{
    public static async Task<int> Main(string[] args)
    {
        string appName = "Winter.Monitor";

        using var _ = new Mutex(false, appName, out bool createdNew);
        if (!createdNew)
        {
            await Console.Out.WriteLineAsync($"{appName} 已启动！");
            await Task.Delay(1000);
            return 1;
        }

        Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Async(c => c.File($"{AppContext.BaseDirectory}/Logs/logs.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14))
            .WriteTo.Async(c => c.Console())
            .CreateLogger();

        try
        {
            var serviceOptions = new ServiceOptions
            {
                Description = "Winter 简易监控。"
            };
            serviceOptions.Linux.Service.Restart = "always";
            serviceOptions.Linux.Service.RestartSec = "10";
            serviceOptions.Windows.DisplayName = appName;
            serviceOptions.Windows.FailureActionType = WindowsServiceActionType.Restart;

            if (Service.UseServiceSelf(args, appName, serviceOptions))
            {
                Log.Information($"Starting {appName}.");

                var builder = Host
                 .CreateDefaultBuilder(args)
                 .UseServiceSelf();

                builder.ConfigureServices(services =>
                {
                    services.AddApplication<WinterMonitorModule>(
                        options => options.Services.ReplaceConfiguration(services.GetConfiguration())
                        );
                }).UseAutofac().UseSerilog();

                var host = builder.Build();

                await host
                    .Services
                    .GetRequiredService<IAbpApplicationWithExternalServiceProvider>()
                    .InitializeAsync(host.Services);

                await host.RunAsync();
            }

            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, $"{appName} terminated unexpectedly!");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
