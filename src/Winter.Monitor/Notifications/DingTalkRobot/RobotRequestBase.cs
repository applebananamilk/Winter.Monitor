using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Volo.Abp;

namespace Winter.Monitor.Notifications.DingTalkRobot;

/// <summary>
/// 请求基类。
/// </summary>
public abstract class RobotRequestBase
{
    /// <summary>
    /// Webhook地址。
    /// </summary>
    private readonly string _webhook;

    /// <summary>
    /// 密钥，机器人安全设置页面，加签一栏下面显示的SEC开头的字符串。如果为空则忽略。
    /// </summary>
    public string? Secret { get; set; }

    public RobotRequestBase(string webhook)
    {
        Check.NotNullOrEmpty(webhook, nameof(webhook));

        _webhook = webhook;
    }

    /// <summary>
    /// 获取Webhook地址。
    /// </summary>
    /// <returns></returns>
    public virtual string GetWebhookUrl()
    {
        if (Secret.IsNullOrWhiteSpace())
        {
            return _webhook;
        }

        var timestamp = new DateTimeOffset(DateTime.Now).UtcDateTime.Ticks / 10000L - 62135596800000L;

        return $"{_webhook}&timestamp={timestamp}&sign={Encrypt(timestamp, Secret!)}";
    }

    protected virtual string Encrypt(long timestamp, string secret)
    {
        string stringToSign = timestamp + "\n" + secret;
        var encoding = new ASCIIEncoding();
        byte[] keyByte = encoding.GetBytes(secret);
        byte[] messageBytes = encoding.GetBytes(stringToSign);
        using var hmacsha256 = new HMACSHA256(keyByte);
        byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
        return HttpUtility.UrlEncode(Convert.ToBase64String(hashmessage), Encoding.UTF8);
    }
}
