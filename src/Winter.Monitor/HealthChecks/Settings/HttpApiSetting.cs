using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Winter.Monitor.HealthChecks.Settings;

/// <summary>
/// HttpApi监控设置。
/// </summary>
public class HttpApiSetting
{
    /// <summary>
    /// 名称。
    /// </summary>
    [Required]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Url。
    /// </summary>
    [Required]
    [Url]
    public string Url { get; set; } = null!;

    /// <summary>
    /// 请求方法。
    /// </summary>
    [Required]
    public string HttpMethod { get; set; } = null!;

    /// <summary>
    /// Headers。
    /// </summary>
    public Dictionary<string, string> Headers { get; set; } = new();

    /// <summary>
    /// Params。
    /// </summary>
    public Dictionary<string, string> Params { get; set; } = new();

    /// <summary>
    /// 告警阈值。
    /// </summary>
    public HttpApiWarningThreshold? WarningThreshold { get; set; }
}

/// <summary>
/// HttpApi告警阈值。
/// </summary>
public class HttpApiWarningThreshold
{
    /// <summary>
    /// 超时时间，单位毫秒。
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Timeout { get; set; }

    /// <summary>
    /// 表示成功状态码，被包含在内的状态码视为正常响应。
    /// </summary>
    public List<HttpStatusCode> SuccessCode { get; set; } = new List<HttpStatusCode>();
}