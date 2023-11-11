using System.Threading;
using System.Threading.Tasks;

namespace Winter.Monitor.Notifications;

/// <summary>
/// 发送通知接口。
/// </summary>
public interface INotificationSender
{
    /// <summary>
    /// 发送通知。
    /// </summary>
    /// <param name="content">内容。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    Task SendAsync(
        string content,
        CancellationToken cancellationToken = default);
}
