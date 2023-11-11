using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using Winter.Monitor.HealthChecks.Implements;

namespace Winter.Monitor.HealthChecks.Extensions;

public static class RedisHealthCheckBuilderExtensions
{
    private const string Name = "redis";

    public static IHealthChecksBuilder AddRedis(
        this IHealthChecksBuilder builder,
        string redisConnectionString,
        string? name = default,
        HealthStatus? failureStatus = default,
        IEnumerable<string>? tags = default,
        TimeSpan? timeout = default)
    {
        if (redisConnectionString == null)
        {
            throw new ArgumentNullException(nameof(redisConnectionString));
        }

        return builder.Add(new HealthCheckRegistration(
           name ?? Name,
           sp => new RedisHealthCheck(redisConnectionString),
           failureStatus,
           tags,
           timeout));
    }
}
