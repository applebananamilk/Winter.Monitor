using CZGL.SystemInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Winter.Monitor.HealthChecks.Settings;

namespace Winter.Monitor.HealthChecks;

/// <summary>
/// 监控选项。
/// </summary>
public class WinterMonitorOptions
{
    private string? _serverName;

    /// <summary>
    /// 服务器名称，默认是计算机名称。
    /// </summary>
    public string ServerName
    {
        get
        {
            if (string.IsNullOrEmpty(_serverName))
            {
                return SystemPlatformInfo.MachineName;
            }
            return _serverName;
        }
        set => _serverName = value;
    }

    /// <summary>
    /// 轮询间隔。
    /// </summary>
    [Range(10, int.MaxValue)]
    public int PollingIntervalInSeconds { get; set; } = 10;

    /// <summary>
    /// 告警设置。
    /// </summary>
    public AlarmSetting AlarmSetting { get; set; } = new();

    /// <summary>
    /// 系统监控设置。
    /// </summary>
    [Required]
    public SystemSetting SystemSetting { get; set; } = new();

    /// <summary>
    /// 数据库监控设置。
    /// </summary>
    public List<DatabaseSetting> DatabaseSettings { get; set; } = new();

    /// <summary>
    /// Ping监控设置。
    /// </summary>
    public List<PingSetting> PingSettings { get; set; } = new();

    /// <summary>
    /// Tcp监控设置。
    /// </summary>
    public List<TcpSetting> TcpSettings { get; set; } = new();
}
