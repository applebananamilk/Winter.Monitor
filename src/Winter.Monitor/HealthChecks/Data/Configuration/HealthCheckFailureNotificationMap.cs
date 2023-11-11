using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Winter.Monitor.HealthChecks.Data.Configuration;

public class HealthCheckFailureNotificationMap : IEntityTypeConfiguration<HealthCheckFailureNotification>
{
    public void Configure(EntityTypeBuilder<HealthCheckFailureNotification> builder)
    {
        builder.ToTable("HealthCheckFailureNotifications");

        builder.Property(p => p.HealthCheckName)
            .HasMaxLength(500)
            .IsRequired(true);
    }
}
