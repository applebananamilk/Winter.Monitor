using System.Text.Json.Serialization;

namespace Winter.Monitor.Notifications.DingTalkRobot;

public class RobotResponse
{
    /// <summary>
    /// 错误代码。
    /// </summary>
    [JsonPropertyName("errcode")]
    public int ErrCode { get; set; } = -1;

    /// <summary>
    /// 错误信息
    /// </summary>
    [JsonPropertyName("errmsg")]
    public string? ErrMsg { get; set; }
}
