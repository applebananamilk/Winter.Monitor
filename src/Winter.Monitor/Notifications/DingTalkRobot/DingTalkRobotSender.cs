using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Winter.Monitor.Notifications.DingTalkRobot;

public class DingTalkRobotSender : IDingTalkRobotSender, ISingletonDependency
{
    protected ILogger<DingTalkRobotSender> Logger { get; }

    protected IHttpClientFactory HttpClientFactory { get; }

    public DingTalkRobotSender(
        ILogger<DingTalkRobotSender> logger,
        IHttpClientFactory httpClientFactory)
    {
        Logger = logger;
        HttpClientFactory = httpClientFactory;
    }

    public virtual async Task<RobotResponse> SendTextAsync(RobotSendTextRequest request, CancellationToken cancellationToken = default)
    {
        Check.NotNull(request, nameof(request));

        var sendMessageRequest = new
        {
            at = new
            {
                atMobiles = request.AtMobiles,
                isAtAll = request.IsAtAll,
            },
            text = new
            {
                content = request.Content
            },
            msgtype = "text"
        };

        using var httpClient = HttpClientFactory.CreateClient(nameof(DingTalkRobotSender));

        HttpContent httpContent = new StringContent(Serialize(sendMessageRequest));
        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var httpResponseMessage = await httpClient.PostAsync(
            requestUri: request.GetWebhookUrl(),
            content: httpContent,
            cancellationToken: cancellationToken);

        httpResponseMessage.EnsureSuccessStatusCode();

        var respJsonString = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

        var robotResponse = Deserialize<RobotResponse>(respJsonString);

        if (robotResponse is not null && robotResponse.ErrCode != 0)
        {
            Logger.LogWarning("钉钉机器人发送消息时失败：{respJsonString}", respJsonString);
        }

        return robotResponse!;
    }

    private static string Serialize(object value)
    {
        Check.NotNull(value, nameof(value));

        return JsonSerializer.Serialize(value);
    }

    private static T? Deserialize<T>(string respJsonString) where T : class
    {
        if (string.IsNullOrEmpty(respJsonString))
        {
            throw new RobotResponseException("钉钉机器人发送请求后响应内容为空！");
        }

        return JsonSerializer.Deserialize<T>(respJsonString);
    }
}
