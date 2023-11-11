using System;
using System.ComponentModel.DataAnnotations;

namespace Winter.Monitor.HealthChecks.Settings;

/// <summary>
/// 数据库监控。
/// </summary>
public class DatabaseSetting
{
    /// <summary>
    /// 名称。
    /// </summary>
    [Required]
    public string Name { get; set; } = null!;

    /// <summary>
    /// 连接字符串。
    /// </summary>
    [Required]
    public string ConnectionString { get; set; } = null!;

    /// <summary>
    /// 数据库类型。
    /// </summary>
    [Required]
    public string DbType { get; set; } = null!;

    /// <summary>
    /// 告警阈值。
    /// </summary>
    public DatabaseWarningThreshold? WarningThreshold { get; set; }
}

/// <summary>
/// 数据库告警阈值。
/// </summary>
public class DatabaseWarningThreshold
{
    /// <summary>
    /// 超时时间，单位毫秒。
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Timeout { get; set; }
}