using System.ComponentModel.DataAnnotations;

namespace Winter.Monitor.HealthChecks.Settings;

/// <summary>
/// Ping监控设置。
/// </summary>
public class PingSetting
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
    /// 告警阈值。
    /// </summary>
    public PingWarningThreshold WarningThreshold { get; set; } = new();
}

/// <summary>
/// Ping告警阈值。
/// </summary>
public class PingWarningThreshold
{
    /// <summary>
    /// 超时时间，单位毫秒。
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Timeout { get; set; } = 2000;
}
