using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.BackgroundWorkers.Quartz;
using Volo.Abp.Modularity;
using Winter.Monitor.BackgroundWorks;
using Winter.Monitor.HealthChecks.Data;
using Winter.Monitor.HealthChecks.Extensions;
using Winter.Monitor.Notifications.DingTalkRobot;
using Winter.Monitor.Notifications.Email;

namespace Winter.Monitor;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpBackgroundWorkersQuartzModule)
    )]
public class WinterMonitorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        // Notifications
        Configure<DingTalkRobotOptions>(configuration.GetSection("Notifications:DingTalkRobot"));
        Configure<EmailOptions>(configuration.GetSection("Notifications:Email"));

        // Periodic Reporting
        Configure<PeriodicReportingWorkerOptions>(configuration.GetSection("PeriodicReportingWorker"));

        context.Services.AddHttpClient();

        // 健康检查监视器相关服务
        context.Services.AddHealthChecksMonitor();

        // Sqlite
        context.Services.AddSqlite<WinterMonitorDbContext>("Data Source=winter_monitor.db");

        // 取消自动注册后台任务。
        Configure<AbpBackgroundWorkerQuartzOptions>(options => options.IsAutoRegisterEnabled = false);
    }

    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        var periodicReportingWorkerOptions = context.ServiceProvider.GetRequiredService<IOptions<PeriodicReportingWorkerOptions>>().Value;

        if (periodicReportingWorkerOptions.IsEnabled)
        {
            await context.AddBackgroundWorkerAsync<PeriodicReportingWorker>();
        }

        // 自动创建数据库
        using (var serviceScope = context.ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<WinterMonitorDbContext>();
            await dbContext.Database.EnsureCreatedAsync();
        }
    }
}
