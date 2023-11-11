using System;

namespace Winter.Monitor.HealthChecks.Exceptions;

/// <summary>
/// 健康检查设置异常。
/// </summary>
public class HealthCheckSettingException : Exception
{
    public HealthCheckSettingException(string message) : base(message)
    {

    }
}
