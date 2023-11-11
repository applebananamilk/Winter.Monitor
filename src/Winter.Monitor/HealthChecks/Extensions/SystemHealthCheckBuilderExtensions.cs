using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using Volo.Abp;
using Winter.Monitor.HealthChecks.Implements;

namespace Winter.Monitor.HealthChecks.Extensions;

public static class SystemHealthCheckBuilderExtensions
{
    private const string DiskName = "disk";
    private const string MemoryName = "sysmem";
    private const string ProcessName = "process";

    public static IHealthChecksBuilder AddSystemDiskHealthCheck(
        this IHealthChecksBuilder builder,
        double maximumUsedDiskPercentage,
        string? name = default,
        HealthStatus? failureStatus = default,
        IEnumerable<string>? tags = default,
        TimeSpan? timeout = default)
    {
        if (maximumUsedDiskPercentage <= 0)
        {
            throw new ArgumentException($"{nameof(maximumUsedDiskPercentage)} should be greater than zero");
        }

        string registrationName = name ?? DiskName;

        return builder.Add(new HealthCheckRegistration(
            registrationName,
            sp => new SystemDiskHealthCheck(maximumUsedDiskPercentage),
            failureStatus,
            tags,
            timeout));
    }

    public static IHealthChecksBuilder AddSystemMemoryHealthCheck(
        this IHealthChecksBuilder builder,
        double maximumUsedMemoryPercentage,
        string? name = default,
        HealthStatus? failureStatus = default,
        IEnumerable<string>? tags = default,
        TimeSpan? timeout = default)
    {
        if (maximumUsedMemoryPercentage <= 0)
        {
            throw new ArgumentException($"{nameof(maximumUsedMemoryPercentage)} should be greater than zero");
        }

        string registrationName = name ?? MemoryName;

        return builder.Add(new HealthCheckRegistration(
            registrationName,
            sp => new SystemMemoryHealthCheck(maximumUsedMemoryPercentage),
            failureStatus,
            tags,
            timeout));
    }

    public static IHealthChecksBuilder AddSystemProcessHealthCheck(
        this IHealthChecksBuilder builder,
        string processName,
        string? name = default,
        HealthStatus? failureStatus = default,
        IEnumerable<string>? tags = default,
        TimeSpan? timeout = default)
    {
        Check.NotNullOrEmpty(processName, nameof(processName));

        return builder.Add(new HealthCheckRegistration(
            name ?? ProcessName,
            sp => new SystemProcessHealthCheck(processName),
            failureStatus,
            tags,
            timeout));
    }
}
