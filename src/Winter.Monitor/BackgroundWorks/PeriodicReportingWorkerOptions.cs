namespace Winter.Monitor.BackgroundWorks;

/// <summary>
/// 定时报告工作者选项。
/// </summary>
public class PeriodicReportingWorkerOptions
{
    /// <summary>
    /// 是否启用。
    /// </summary>
    public bool IsEnabled { get; set; } = false;

    /// <summary>
    /// 立即执行。
    /// </summary>
    public bool StartNow { get; set; } = false;

    /// <summary>
    /// 定时通知Cron表达式，默认每月15日上午10:15触发。
    /// </summary>
    public string Cron { get; set; } = "0 15 10 15 * ?";
}
