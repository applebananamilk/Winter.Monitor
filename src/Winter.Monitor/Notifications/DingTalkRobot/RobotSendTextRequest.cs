using System.Collections.Generic;
using Volo.Abp;

namespace Winter.Monitor.Notifications.DingTalkRobot;

/// <summary>
/// 发送文本请求实体。
/// </summary>
public class RobotSendTextRequest : RobotRequestBase
{
    /// <summary>
    /// 消息内容。
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// 被@人的手机号。
    /// </summary>
    public List<string> AtMobiles { get; set; }

    /// <summary>
    /// 是否@所有人。
    /// </summary>
    public bool IsAtAll { get; set; }

    public RobotSendTextRequest(string webhook, string content) : base(webhook)
    {
        Check.NotNullOrEmpty(content, nameof(content));

        Content = content;
        AtMobiles = new List<string>();
    }

    public RobotSendTextRequest(string webhook, string content, List<string> atMobiles, bool isAtAll)
        : this(webhook, content)
    {
        Content = content;
        AtMobiles = atMobiles;
        IsAtAll = isAtAll;
    }
}
