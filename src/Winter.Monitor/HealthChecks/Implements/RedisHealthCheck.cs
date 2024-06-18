using FreeRedis;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Winter.Monitor.HealthChecks.Implements;

public class RedisHealthCheck : IHealthCheck
{
    private static readonly ConcurrentDictionary<string, RedisClient> _connections = new();
    private readonly string _redisConnectionString;

    public RedisHealthCheck(string redisConnectionString)
    {
        _redisConnectionString = redisConnectionString ?? throw new ArgumentNullException(nameof(redisConnectionString));
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_connections.TryGetValue(_redisConnectionString, out var connection))
            {
                connection = new RedisClient(_redisConnectionString);
                _connections[_redisConnectionString] = connection;
            }

            connection.Ping();

            return Task.FromResult(HealthCheckResult.Healthy(GetDescription(connection)));
        }
        catch (Exception ex)
        {
            return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, exception: ex));
        }
    }

    private static string? GetDescription(RedisClient connection)
    {
        try
        {
            string redisMemoryInfo = connection.Info("Memory");
            int memoryIndex = redisMemoryInfo.IndexOf("used_memory_human", StringComparison.Ordinal);
            if (memoryIndex >= 0)
            {
                int firstNewLineIndex = redisMemoryInfo.IndexOf(Environment.NewLine, memoryIndex, StringComparison.Ordinal);
                return redisMemoryInfo[memoryIndex..firstNewLineIndex];
            }
        }
        catch (Exception)
        {
            // Ignore exception
        }

        return null;
    }
}
