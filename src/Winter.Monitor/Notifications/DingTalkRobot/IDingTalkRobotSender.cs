using System.Threading;
using System.Threading.Tasks;

namespace Winter.Monitor.Notifications.DingTalkRobot;

public interface IDingTalkRobotSender
{
    Task<RobotResponse> SendTextAsync(RobotSendTextRequest request, CancellationToken cancellationToken = default);
}
