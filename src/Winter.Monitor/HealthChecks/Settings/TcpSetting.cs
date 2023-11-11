using System;
using System.ComponentModel.DataAnnotations;

namespace Winter.Monitor.HealthChecks.Settings;

/// <summary>
/// Tcp监控设置。
/// </summary>
public class TcpSetting
{
    /// <summary>
    /// 名称。
    /// </summary>
    [Required]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Host。
    /// </summary>
    [Required]
    public string Host { get; set; } = null!;

    /// <summary>
    /// 端口。
    /// </summary>
    [Range(1, 65535)]
    public int Port { get; set; }

    /// <summary>
    /// 告警阈值。
    /// </summary>
    public TcpWarningThreshold? WarningThreshold { get; set; }
}

/// <summary>
/// Tcp告警阈值。
/// </summary>
public class TcpWarningThreshold
{
    /// <summary>
    /// 超时时间，单位毫秒。
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Timeout { get; set; }
}
