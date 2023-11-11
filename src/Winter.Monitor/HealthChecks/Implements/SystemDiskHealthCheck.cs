using CZGL.SystemInfo;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Winter.Monitor.HealthChecks.Implements;

public class SystemDiskHealthCheck : IHealthCheck
{
    private readonly double _maximumUsedDiskPercentage;

    public SystemDiskHealthCheck(double maximumUsedDiskPercentage)
    {
        _maximumUsedDiskPercentage = maximumUsedDiskPercentage;
    }

    /// <inheritdoc/>
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            List<string> errorList = new();
            List<string> reportList = new();

            foreach (var disk in DiskInfo.GetRealDisk())
            {
                reportList.Add($"名称：{disk.Name}，总量：{ConvertBytesToMB(disk.TotalSize)}MB，剩余：{ConvertBytesToMB(disk.FreeSpace)}MB，已使用：{CalculatePercentage(disk.UsedSize, disk.TotalSize)}%");

                double p = CalculatePercentage(disk.UsedSize, disk.TotalSize);

                if (p > _maximumUsedDiskPercentage)
                {
                    errorList.Add($"磁盘[{disk.Name}]触发告警阈值[{_maximumUsedDiskPercentage}%]，剩余[{ConvertBytesToMB(disk.FreeSpace)}]MB");
                }
            }

            if (errorList.Count == 0)
            {
                return Task.FromResult(HealthCheckResult.Healthy(string.Join("; ", reportList)));
            }

            return Task.FromResult(
                new HealthCheckResult(context.Registration.FailureStatus, description: string.Join("; ", errorList))
                );
        }
        catch (Exception ex)
        {
            return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, exception: ex));
        }
    }

    private static long ConvertBytesToMB(long bytes)
    {
        if (bytes <= 0)
        {
            throw new ArgumentException(nameof(bytes) + "不能小于等于0！");
        }

        return bytes / 1024 / 1024;
    }

    private static double CalculatePercentage(long n1, long n2)
    {
        if (n1 <= 0)
        {
            throw new ArgumentException(nameof(n1) + "不能小于等于0！");
        }

        if (n2 <= 0)
        {
            throw new ArgumentException(nameof(n2) + "不能小于等于0！");
        }

        return Math.Round((double)n1 / n2 * 100, 2);
    }
}