using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Winter.Monitor.HealthChecks.Settings;

/// <summary>
/// 系统监控设置。
/// </summary>
public class SystemSetting
{
    /// <summary>
    /// 进程监控设置。
    /// </summary>
    public List<ProcessSetting> ProcessSettings { get; set; } = new(0);

    /// <summary>
    /// 告警阈值。
    /// </summary>
    [Required]
    public SystemWarningThreshold WarningThreshold { get; set; } = null!;
}

/// <summary>
/// 告警阈值。
/// </summary>
public class SystemWarningThreshold
{
    /// <summary>
    /// 磁盘，百分比。
    /// </summary>
    [Range(1, 100)]
    public double Disk { get; set; }

    /// <summary>
    /// 内存，百分比。
    /// </summary>
    [Range(1, 100)]
    public double Memory { get; set; }
}