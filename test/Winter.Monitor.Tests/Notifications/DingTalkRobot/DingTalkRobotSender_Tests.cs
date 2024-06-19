using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Winter.Monitor.Notifications.DingTalkRobot;

public class DingTalkRobotSender_Tests : WinterMonitorTestBase
{
    private readonly IDingTalkRobotSender _dingTalkRobotSender;

    private IHttpClientFactory _httpClientFactory = null!;

    public DingTalkRobotSender_Tests()
    {
        var httpClient = Substitute.For<HttpClient>();

        Substitute
            .For<IHttpClientFactory>()
            .CreateClient(nameof(DingTalkRobotSender))
            .Returns(httpClient);

        _dingTalkRobotSender = GetRequiredService<IDingTalkRobotSender>();
    }

    protected override void AfterAddApplication(IServiceCollection services)
    {
        _httpClientFactory = Substitute.For<IHttpClientFactory>();
        services.AddSingleton(_httpClientFactory);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void SendText_Content_IsNull_ThrowsArgumentException(string? content)
    {
        Assert.Throws<ArgumentException>(
            () => new RobotSendTextRequest("https://oapi.dingtalk.com/robot/send?access_token=XXX", content!));
    }

    [Fact]
    public async Task SendText_Request_IsNull_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _dingTalkRobotSender.SendTextAsync(null!));
    }

    [Fact]
    public async Task SendText_Request_If_Not_SuccessStatusCode_Throws_HttpRequestException()
    {
        var req = new RobotSendTextRequest("https://oapi.dingtalk.com/robot/send?access_token=XXX", "你好");

        var httpClient = Substitute.For<HttpClient>();

        httpClient
           .PostAsync("", default, default)
           .ReturnsForAnyArgs(new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden));

        _httpClientFactory.CreateClient(nameof(DingTalkRobotSender)).Returns(httpClient);

        await Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            await _dingTalkRobotSender.SendTextAsync(req);
        });
    }

    [Fact]
    public async Task SendText_Request_If_ResponseContent_Is_Null_Throws_RobotResponseException()
    {
        var req = new RobotSendTextRequest("https://oapi.dingtalk.com/robot/send?access_token=XXX", "你好");

        var httpClient = Substitute.For<HttpClient>();

        httpClient
           .PostAsync("", default, default)
           .ReturnsForAnyArgs(new HttpResponseMessage() { Content = new StringContent("") });

        _httpClientFactory.CreateClient(nameof(DingTalkRobotSender)).Returns(httpClient);

        var exception = await Assert.ThrowsAsync<RobotResponseException>(async () =>
        {
            await _dingTalkRobotSender.SendTextAsync(req);
        });

        exception.Message.ShouldBe("钉钉机器人发送请求后响应内容为空！");
    }
}
