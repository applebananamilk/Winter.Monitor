using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;

namespace Winter.Monitor.HealthChecks.Implements;

public class SystemProcessHealthCheck : IHealthCheck
{
    private readonly string _processName;

    public SystemProcessHealthCheck(string processName)
    {
        _processName = Check.NotNullOrEmpty(processName, nameof(processName));
    }

    /// <inheritdoc />
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var processes = Process.GetProcessesByName(_processName);

            if (processes.Any())
            {
                return Task.FromResult(HealthCheckResult.Healthy());
            }
        }
        catch (Exception ex)
        {
            return Task.FromResult(new HealthCheckResult(HealthStatus.Unhealthy, exception: ex));
        }

        return Task.FromResult(
            new HealthCheckResult(context.Registration.FailureStatus,
            description: $"未找到正在运行的[{_processName}]进程。"));
    }
}
