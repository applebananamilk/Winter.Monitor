using Shouldly;
using System;
using Xunit;

namespace Winter.Monitor.Notifications.DingTalkRobot;

public class RobotRequestBase_Tests
{
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Webhook_IsEmptyOrNull_ThrowsArgumentException(string? webhook)
    {
        Assert.Throws<ArgumentException>(() => new TestRequest(webhook!));
    }

    [Fact]
    public void GetWebhookUrl_All_Test()
    {
        string webhook = "https://oapi.dingtalk.com/robot/send?access_token=XXX";

        var testRequest = new TestRequest(webhook);

        testRequest.GetWebhookUrl().ShouldBe(webhook);

        testRequest.Secret = "ASDAS231SALOJX";

        testRequest.GetWebhookUrl().ShouldNotBe(webhook);
        testRequest.GetWebhookUrl().ShouldContain("sign");
        testRequest.GetWebhookUrl().ShouldContain("timestamp");
    }
}

class TestRequest(string webhook) : RobotRequestBase(webhook)
{
}
