using System.Collections.Generic;

namespace Winter.Monitor.Notifications.DingTalkRobot;

/// <summary>
/// 钉钉机器人配置。
/// </summary>
public class DingTalkRobotOptions
{
    /// <summary>
    /// 是否启用。
    /// </summary>
    public bool IsEnabled { get; set; } = false;

    /// <summary>
    /// Webhook地址，为空不会发送。
    /// </summary>
    public string? Webhook { get; set; }

    /// <summary>
    /// 被@的人的手机号。
    /// </summary>
    public List<string> AtMobiles { get; set; } = new();

    /// <summary>
    /// 是否@所有人。
    /// </summary>
    public bool IsAtAll { get; set; }
}
