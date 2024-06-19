using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp;

namespace Winter.Monitor.HealthChecks;

internal static class HealthCheckHelper
{
    internal static class Group
    {
        public const string OS = "OS";
        public const string Process = "Process";
        public const string DB = "DB";
        public const string Ping = "Ping";
        public const string Tcp = "Tcp";
    }

    /// <summary>
    /// 转时间段。
    /// </summary>
    /// <param name="milliseconds"></param>
    /// <returns></returns>
    public static TimeSpan? ToTimeSpan(int? milliseconds)
    {
        if (milliseconds.HasValue && milliseconds >= 0)
        {
            return TimeSpan.FromMilliseconds(milliseconds.Value);
        }

        return null;
    }

    /// <summary>
    /// 分组分割符号。
    /// </summary>
    public const char GroupSeparator = '@';

    /// <summary>
    /// 计算健康检查项名称。
    /// </summary>
    /// <param name="groupName">分组命名。</param>
    /// <param name="name">名称。</param>
    /// <returns></returns>
    public static string CalculateHealthCheckName(string groupName, string name)
    {
        Check.NotNullOrEmpty(groupName, nameof(groupName));
        Check.NotNullOrEmpty(name, nameof(name));

        return groupName + GroupSeparator + name;
    }

    public static (string? GroupName, string Name) ResolveHealthCheckName(string healthCheckName)
    {
        Check.NotNullOrEmpty(healthCheckName, nameof(healthCheckName));

        int index = healthCheckName.IndexOf(GroupSeparator, StringComparison.Ordinal);

        string? groupName = null;

        if (index != -1)
        {
            groupName = healthCheckName[..index];
        }

        string name = healthCheckName[(index + 1)..];

        return (groupName, name);
    }

    /// <summary>
    /// 健康状态Emoji映射。
    /// </summary>
    private static readonly Dictionary<HealthStatus, string> HealthStatusEmojiMap = new()
    {
        { HealthStatus.Healthy , "✅" },
        { HealthStatus.Degraded , "❗" },
        { HealthStatus.Unhealthy , "❌" },
    };

    /// <summary>
    /// 生成报告。
    /// </summary>
    /// <param name="entries">报告条目。</param>
    /// <returns>报告内容。</returns>
    public static string GenerateReport(
        IEnumerable<KeyValuePair<string, HealthReportEntry>> entries)
    {
        StringBuilder reportContentBuilder = new StringBuilder();

        var groupReportDictionary = new Dictionary<string, List<string>>();

        foreach (var entry in entries)
        {
            (string? groupName, string name) = ResolveHealthCheckName(entry.Key);
            if (string.IsNullOrEmpty(groupName))
            {
                groupName = "Others";
            }
            var items = groupReportDictionary.GetOrAdd(groupName, () => new List<string>());
            items.Add($"   - {name}");
            items.Add($"      HealthStatus : {entry.Value.Status} {HealthStatusEmojiMap[entry.Value.Status]}");
            if (!string.IsNullOrEmpty(entry.Value.Description))
            {
                items.Add($"      Description : {entry.Value.Description}");
            }
            if (entry.Value.Exception != null)
            {
                items.Add($"      Exception : {entry.Value.Exception.Message}");
            }
        }

        foreach (var group in groupReportDictionary)
        {
            reportContentBuilder.AppendLine(group.Key);

            foreach (string groupReportEntry in group.Value)
            {
                reportContentBuilder.AppendLine(groupReportEntry);
            }
        }

        return reportContentBuilder.ToString();
    }
}
