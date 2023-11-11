using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Winter.Monitor.HealthChecks.Settings;

/// <summary>
/// 告警设置。
/// </summary>
public class AlarmSetting
{
    /// <summary>
    /// 恢复通知设置。
    /// </summary>
    public AlarmRecoveryNotification RecoveryNotification { get; set; } = new();

    /// <summary>
    /// 告警收敛设置。
    /// </summary>
    public AlarmConvergence Convergence { get; set; } = new();

    /// <summary>
    /// 告警静默设置。
    /// </summary>
    public AlarmSilence Silence { get; set; } = new();
}

/// <summary>
/// 告警恢复
/// </summary>
public class AlarmRecoveryNotification
{
    /// <summary>
    /// 是否启用。
    /// </summary>
    public bool IsEnabled { get; set; } = true;
}

/// <summary>
/// 告警收敛。
/// </summary>
public class AlarmConvergence
{
    /// <summary>
    /// 是否启用。
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 重复告警收敛周期（秒）。
    /// </summary>
    [Range(30, int.MaxValue)]
    public int EvalInterval { get; set; } = 600;
}

/// <summary>
/// 告警静默。
/// </summary>
public class AlarmSilence : IValidatableObject
{
    /// <summary>
    /// 是否启用。
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// 是否对所有检查项生效。
    /// </summary>
    public bool MatchAll { get; set; } = true;

    /// <summary>
    /// 生效检查名字。
    /// </summary>
    public List<string> HealthCheckNames { get; set; } = new List<string>();

    /// <summary>
    /// 静默时段开始。
    /// </summary>
    public string PeriodStart { get; set; } = "0:00";

    /// <summary>
    /// 静默时段结束。
    /// </summary>
    public string PeriodEnd { get; set; } = "0:00";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (IsEnabled)
        {
            if (!TimeOnly.TryParse(PeriodStart, out _))
            {
                yield return new ValidationResult("时间格式错误！", new[] { nameof(PeriodStart) });
            }

            if (!TimeOnly.TryParse(PeriodEnd, out _))
            {
                yield return new ValidationResult("时间格式错误！", new[] { nameof(PeriodEnd) });
            }
        }
    }
}