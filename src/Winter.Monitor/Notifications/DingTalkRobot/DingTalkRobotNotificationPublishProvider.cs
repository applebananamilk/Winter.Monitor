using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace Winter.Monitor.Notifications.DingTalkRobot;

public class DingTalkRobotNotificationPublishProvider : INotificationPublishProvider
{
    private readonly IDingTalkRobotSender _dingTalkRobotSender;
    private readonly DingTalkRobotOptions _dingTalkRobotOptions;

    public DingTalkRobotNotificationPublishProvider(
        IDingTalkRobotSender dingTalkRobotSender,
        IOptionsMonitor<DingTalkRobotOptions> dingTalkRobotOptionsAccessor)
    {
        _dingTalkRobotSender = dingTalkRobotSender;
        _dingTalkRobotOptions = dingTalkRobotOptionsAccessor.CurrentValue;
    }

    public async Task PublishAsync(string content, CancellationToken cancellationToken = default)
    {
        if (_dingTalkRobotOptions.IsEnabled && !string.IsNullOrEmpty(_dingTalkRobotOptions.Webhook))
        {
            var sendTextRequest = new RobotSendTextRequest(
                _dingTalkRobotOptions.Webhook,
                content,
                _dingTalkRobotOptions.AtMobiles,
                _dingTalkRobotOptions.IsAtAll);

            await _dingTalkRobotSender.SendTextAsync(sendTextRequest, cancellationToken);
        }
    }
}
