using System;

namespace Winter.Monitor.Notifications.DingTalkRobot;

public class RobotResponseException : Exception
{
    public RobotResponseException(string message) : base(message)
    {

    }
}
