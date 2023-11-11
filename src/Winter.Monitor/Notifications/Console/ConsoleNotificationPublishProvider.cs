using System.Threading;
using System.Threading.Tasks;

namespace Winter.Monitor.Notifications.Console;

#if DEBUG
public class ConsoleNotificationPublishProvider : INotificationPublishProvider
{
    public async Task PublishAsync(string content, CancellationToken cancellationToken = default)
    {
        await System.Console.Out.WriteLineAsync(content);
    }
}
#endif
