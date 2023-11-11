using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Winter.Monitor.HealthChecks.Data;
using Winter.Monitor.HealthChecks.Settings;
using Winter.Monitor.Notifications;

namespace Winter.Monitor.HealthChecks.Publishers;

internal sealed class HealthCheckPublisher : IHealthCheckPublisher
{
    private readonly WinterMonitorOptions _monitorOptions;
    private readonly WinterMonitorDbContext _winterMonitorDb;
    private readonly INotificationSender _notificationSender;

    public HealthCheckPublisher(
        IOptions<WinterMonitorOptions> monitorOptionsAccessor,
        WinterMonitorDbContext winterMonitorDb,
        INotificationSender notificationSender)
    {
        _monitorOptions = monitorOptionsAccessor.Value;
        _winterMonitorDb = winterMonitorDb;
        _notificationSender = notificationSender;
    }

    public async Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
    {
        TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.Now);
        AlarmSilence silence = _monitorOptions.AlarmSetting.Silence;

        if (!silence.IsEnabled)
        {
            await SendFailedAlarmNotificationAsync(report.Entries);
        }
        else
        {
            // 启用告警静默处理。
            var entries = report.Entries.ToList();
            TimeOnly periodStart = TimeOnly.Parse(silence.PeriodStart);
            TimeOnly periodEnd = TimeOnly.Parse(silence.PeriodEnd);

            if (silence.MatchAll)
            {
                // 如果全部检查都启用静默处理，则在静默时段之外发送。
                if (currentTime < periodStart || currentTime > periodEnd)
                {
                    await SendFailedAlarmNotificationAsync(entries);
                }
            }
            else
            {
                // 如果匹配指定检查名字静默，则在静默时间之内需要过滤掉该检查项。
                if (currentTime >= periodStart && currentTime <= periodEnd)
                {
                    entries = entries.Where(p => !silence.HealthCheckNames.Contains(p.Key)).ToList();
                }

                await SendFailedAlarmNotificationAsync(entries);
            }
        }

        // 设计上，不论如何都会调用恢复通知的方法，防止发生静默前发送了失败通知，但是之后恢复了却不知道的情况。
        await SendRecoveredNotificationAsync(report.Entries);
    }

    private async Task SendFailedAlarmNotificationAsync(IEnumerable<KeyValuePair<string, HealthReportEntry>> entries)
    {
        // 过滤出异常条目
        var unhealthyEntries = entries.Where(p => p.Value.Status != HealthStatus.Healthy).ToList();

        // 获取所有已通知数据
        var failureNotifications = await GetFailureNotificationListAsync();

        // 过滤进入收敛状态的条目
        if (_monitorOptions.AlarmSetting.Convergence.IsEnabled)
        {
            var healthCheckNames = unhealthyEntries.Select(p => p.Key);

            DateTime startTime = DateTime.Now.AddSeconds(-_monitorOptions.AlarmSetting.Convergence.EvalInterval);

            var nonSendNotificationNames = failureNotifications
                .Where(p => healthCheckNames.Contains(p.HealthCheckName) && !p.IsUpAndRunning && p.LastNotified >= startTime)
                .Select(p => p.HealthCheckName);

            unhealthyEntries = unhealthyEntries.Where(p => !nonSendNotificationNames.Contains(p.Key)).ToList();
        }

        // 发送告警通知
        if (unhealthyEntries.Any())
        {
            var msgBuilder = new StringBuilder();
            msgBuilder.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {_monitorOptions.ServerName} 健康检查异常通知");
            msgBuilder.Append(HealthCheckHelper.GenerateReport(unhealthyEntries));

            await _notificationSender.SendAsync(msgBuilder.ToString());

            // 保存发送记录
            await SaveFailureNotificationAsync(failureNotifications, unhealthyEntries, false);
        }
    }

    private async Task SendRecoveredNotificationAsync(IEnumerable<KeyValuePair<string, HealthReportEntry>> entries)
    {
        if (!_monitorOptions.AlarmSetting.RecoveryNotification.IsEnabled)
        {
            return;
        }

        // 过滤出正常条目
        var healthyEntries = entries.Where(p => p.Value.Status == HealthStatus.Healthy).ToList();

        // 获取所有已通知数据
        var failureNotifications = await GetFailureNotificationListAsync();

        // 判断是否有异常已恢复
        List<string> recoveredNames = new List<string>();
        foreach (var healthyEntry in healthyEntries)
        {
            var failureNotification = failureNotifications.Find(p => p.HealthCheckName == healthyEntry.Key);

            // 5天内状态是未恢复运行的才发送通知。
            if (failureNotification is { IsUpAndRunning: false } &&
                failureNotification.LastNotified >= DateTime.Now.AddDays(-5))
            {
                recoveredNames.Add(healthyEntry.Key);
            }
        }

        healthyEntries = healthyEntries.Where(p => recoveredNames.Contains(p.Key)).ToList();

        // 发送恢复通知
        if (healthyEntries.Any())
        {
            var msgBuilder = new StringBuilder();
            msgBuilder.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {_monitorOptions.ServerName} 健康检查恢复通知");
            msgBuilder.Append(HealthCheckHelper.GenerateReport(healthyEntries));

            await _notificationSender.SendAsync(msgBuilder.ToString());

            // 保存发送记录
            await SaveFailureNotificationAsync(failureNotifications, healthyEntries, true);
        }
    }

    private async Task<List<HealthCheckFailureNotification>> GetFailureNotificationListAsync()
    {
        return await _winterMonitorDb.Failures.ToListAsync();
    }

    private async Task SaveFailureNotificationAsync(
        List<HealthCheckFailureNotification> failureNotifications,
        IEnumerable<KeyValuePair<string, HealthReportEntry>> entries,
        bool isUpAndRunning)
    {
        foreach (var entry in entries)
        {
            var failureNotification = failureNotifications.Find(p => p.HealthCheckName == entry.Key);

            if (failureNotification != null)
            {
                failureNotification.LastNotified = DateTime.Now;
                failureNotification.IsUpAndRunning = isUpAndRunning;

                _winterMonitorDb.Update(failureNotification);
            }
            else
            {
                failureNotification = new HealthCheckFailureNotification
                {
                    HealthCheckName = entry.Key,
                    IsUpAndRunning = isUpAndRunning,
                    LastNotified = DateTime.Now
                };

                await _winterMonitorDb.AddAsync(failureNotification);
            }
        }

        await _winterMonitorDb.SaveChangesAsync();
    }
}
