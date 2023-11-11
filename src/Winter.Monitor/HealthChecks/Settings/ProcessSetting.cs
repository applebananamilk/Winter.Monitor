using System.ComponentModel.DataAnnotations;

namespace Winter.Monitor.HealthChecks.Settings;

/// <summary>
/// 进程监控设置。
/// </summary>
public class ProcessSetting
{
    /// <summary>
    /// 进程名称。
    /// </summary>
    [Required]
    public string ProcessName { get; set; } = null!;
}
