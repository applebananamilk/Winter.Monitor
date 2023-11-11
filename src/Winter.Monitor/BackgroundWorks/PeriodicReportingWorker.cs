using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers.Quartz;
using Winter.Monitor.HealthChecks;
using Winter.Monitor.Notifications;

namespace Winter.Monitor.BackgroundWorks;

/// <summary>
/// 定时报告。
/// </summary>
[DisallowConcurrentExecution]
public class PeriodicReportingWorker : QuartzBackgroundWorkerBase
{
    private readonly INotificationSender _notificationSender;
    private readonly HealthCheckService _healthCheckService;
    private readonly WinterMonitorOptions _monitorOptions;

    public PeriodicReportingWorker(
        INotificationSender notificationSender,
        HealthCheckService healthCheckService,
        IOptions<WinterMonitorOptions> monitorOptionsAccessor,
        IOptions<PeriodicReportingWorkerOptions> workerOptionsAccessor)
    {
        _notificationSender = notificationSender;
        _healthCheckService = healthCheckService;
        _monitorOptions = monitorOptionsAccessor.Value;

        var workerOptions = workerOptionsAccessor.Value;

        JobDetail = JobBuilder
            .Create<PeriodicReportingWorker>()
            .WithIdentity(nameof(PeriodicReportingWorker))
            .Build();

        Trigger = TriggerBuilder
            .Create()
            .WithIdentity(nameof(PeriodicReportingWorker))
            .WithCronSchedule(workerOptions.Cron)
            .Build();

        if (workerOptions.StartNow)
        {
            Execute(null!).GetAwaiter().GetResult();
        }
    }

    public sealed override async Task Execute(IJobExecutionContext context)
    {
        var report = await _healthCheckService.CheckHealthAsync();

        var msgBuilder = new StringBuilder();
        msgBuilder.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {_monitorOptions.ServerName} 健康检查报告");
        msgBuilder.Append(HealthCheckHelper.GenerateReport(report.Entries));

        await _notificationSender.SendAsync(msgBuilder.ToString());
    }
}
