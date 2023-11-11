using Microsoft.EntityFrameworkCore;
using Winter.Monitor.HealthChecks.Data.Configuration;

namespace Winter.Monitor.HealthChecks.Data;

/// <summary>
/// 监控数据库上下文。
/// </summary>
public class WinterMonitorDbContext : DbContext
{
    /// <summary>
    /// 健康检查失败通知。
    /// </summary>
    public DbSet<HealthCheckFailureNotification> Failures { get; set; } = null!;

    public WinterMonitorDbContext(DbContextOptions<WinterMonitorDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new HealthCheckFailureNotificationMap());
    }
}
