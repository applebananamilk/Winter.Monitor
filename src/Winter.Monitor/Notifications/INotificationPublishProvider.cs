using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Winter.Monitor.Notifications;

/// <summary>
/// 通知发布提供者接口。
/// </summary>
public interface INotificationPublishProvider : ITransientDependency
{
    /// <summary>
    /// 发布通知。
    /// </summary>
    /// <param name="content">内容。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    Task PublishAsync(
        string content,
        CancellationToken cancellationToken = default);
}
