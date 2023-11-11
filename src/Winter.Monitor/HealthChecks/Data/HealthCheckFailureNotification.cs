using System;

namespace Winter.Monitor.HealthChecks.Data;

/// <summary>
/// 健康检查失败通知。
/// </summary>
public class HealthCheckFailureNotification
{
    /// <summary>
    /// Id。
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 健康检查名称。
    /// </summary>
    public string HealthCheckName { get; set; } = null!;

    /// <summary>
    /// 最后更新时间。
    /// </summary>
    public DateTime LastNotified { get; set; }

    /// <summary>
    /// 是否恢复运行。
    /// </summary>
    public bool IsUpAndRunning { get; set; }
}
