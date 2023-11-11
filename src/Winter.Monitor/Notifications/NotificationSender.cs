using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Winter.Monitor.Notifications;

public class NotificationSender : INotificationSender, ITransientDependency
{
    private readonly IEnumerable<INotificationPublishProvider> _providers;
    private readonly ILogger<NotificationSender> _logger;

    public NotificationSender(
        IEnumerable<INotificationPublishProvider> providers,
        ILogger<NotificationSender> logger)
    {
        _providers = providers;
        _logger = logger;
    }

    public async Task SendAsync(string content, CancellationToken cancellationToken = default)
    {
        foreach (var provider in _providers)
        {
            try
            {
                await provider.PublishAsync(content, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
        }
    }
}
