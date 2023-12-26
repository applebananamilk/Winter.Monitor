using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MiniValidation;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Winter.Monitor.HealthChecks.Exceptions;
using Winter.Monitor.HealthChecks.Publishers;

namespace Winter.Monitor.HealthChecks.Extensions;

/// <summary>
/// 扩展方法。
/// </summary>
public static class HealthCheckServiceCollectionExtensions
{
    /// <summary>
    /// 添加健康检查监视相关服务。
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IHealthChecksBuilder AddHealthChecksMonitor(this IServiceCollection services)
    {
        var configuration = services.GetConfiguration();

        // MonitorOptions
        var monitorConfigurationSection = configuration.GetSection("Monitor");

        services.PreConfigure<WinterMonitorOptions>(options =>
        {
            monitorConfigurationSection.Bind(options);

            // Validate the Settings
            if (!MiniValidator.TryValidate(options, out var errors))
            {
                StringBuilder errorMessageBuilder = new();
                errorMessageBuilder.AppendLine("监控选项设置有错误！");

                foreach (var entry in errors)
                {
                    errorMessageBuilder.AppendLine($"  {entry.Key}:");
                    foreach (string error in entry.Value)
                    {
                        errorMessageBuilder.AppendLine($"  - {error}");
                    }
                }

                throw new ValidationException(errorMessageBuilder.ToString());
            }
        });

        // 上面的PreConfigure用于配置HealthCheck相关服务时使用，并执行验证规则。
        services.Configure<WinterMonitorOptions>(monitorConfigurationSection);

        // 通过预配置方式获取监控相关设置。
        var monitorOptions = services.ExecutePreConfiguredActions<WinterMonitorOptions>();

        // HealthChecks
        IHealthChecksBuilder builder = services.AddHealthChecks();

        // PublisherOptions
        services.Configure<HealthCheckPublisherOptions>(options =>
        {
            options.Period = TimeSpan.FromSeconds(monitorOptions.PollingIntervalInSeconds);
            options.Timeout = TimeSpan.FromSeconds(180);
        });

        // Publisher
        services.TryAddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();

        // System
        builder.AddSystemDiskHealthCheck(monitorOptions.SystemSetting.WarningThreshold.Disk, HealthCheckHelper.CalculateName("OS", "Disk"));
        builder.AddSystemMemoryHealthCheck(monitorOptions.SystemSetting.WarningThreshold.Memory, HealthCheckHelper.CalculateName("OS", "Memory"));

        // Process
        foreach (var processSetting in monitorOptions.SystemSetting.ProcessSettings)
        {
            builder.AddSystemProcessHealthCheck(processSetting.ProcessName, HealthCheckHelper.CalculateName("Process", processSetting.ProcessName));
        }

        // Database
        foreach (var database in monitorOptions.DatabaseSettings)
        {
            switch (database.DbType)
            {
                case "MySQL":
                    builder.AddMySql(
                        connectionString: database.ConnectionString,
                        HealthCheckHelper.CalculateName("DB", database.Name),
                        timeout: HealthCheckHelper.ToTimeSpan(database.WarningThreshold?.Timeout));
                    break;
                case "Redis":
                    builder.AddRedis(
                        redisConnectionString: database.ConnectionString,
                        name: HealthCheckHelper.CalculateName("DB", database.Name),
                        timeout: HealthCheckHelper.ToTimeSpan(database.WarningThreshold?.Timeout));
                    break;
                case "SqlServer":
                    builder.AddSqlServer(
                        connectionString: database.ConnectionString,
                        name: HealthCheckHelper.CalculateName("DB", database.Name),
                        timeout: HealthCheckHelper.ToTimeSpan(database.WarningThreshold?.Timeout));
                    break;
                case "MongoDB":
                    builder.AddMongoDb(
                        mongodbConnectionString: database.ConnectionString,
                        name: HealthCheckHelper.CalculateName("DB", database.Name),
                        timeout: HealthCheckHelper.ToTimeSpan(database.WarningThreshold?.Timeout));
                    break;
                default:
                    throw new HealthCheckSettingException($"暂未支持 {database.DbType} 类型的数据库！");
            }
        }

        // Ping
        foreach (var pingSetting in monitorOptions.PingSettings)
        {
            builder.AddPingHealthCheck(
                options =>
                    options.AddHost(pingSetting.Host, pingSetting.WarningThreshold!.Timeout),
                    HealthCheckHelper.CalculateName("Ping", pingSetting.Name),
                    timeout: HealthCheckHelper.ToTimeSpan(pingSetting.WarningThreshold?.Timeout));
        }

        // Tcp
        foreach (var tcpSetting in monitorOptions.TcpSettings)
        {
            builder.AddTcpHealthCheck(
                options =>
                    options.AddHost(tcpSetting.Host, tcpSetting.Port),
                    HealthCheckHelper.CalculateName("Tcp", tcpSetting.Name),
                    timeout: HealthCheckHelper.ToTimeSpan(tcpSetting.WarningThreshold?.Timeout));
        }

        return builder;
    }
}
