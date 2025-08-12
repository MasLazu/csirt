using System.Text.Json.Serialization;

namespace MeUi.Api.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Errors { get; set; }
    public string? ErrorCode { get; set; }
    public T? Data { get; set; }
}