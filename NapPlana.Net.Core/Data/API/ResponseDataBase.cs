using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NapPlana.Core.Data.API;

public class ResponseDataBase
{
    
}

/// <summary>
/// 用于在ws请求中标识全局请求
/// </summary>
public class WsGlobalRequest
{
    [JsonPropertyName("action")]
    public ApiActionType Action { get; set; } = ApiActionType.None;
    
    [JsonPropertyName("params")]
    public object? Params { get; set; }
    
    /// <summary>
    /// 用于标识请求的唯一ID
    /// </summary>
    [JsonPropertyName("echo")]
    public string Echo { get; set; } = string.Empty;
}