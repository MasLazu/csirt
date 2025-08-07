using System.Text.Json.Serialization;

namespace MeUi.Api.Models;

public class ErrorResponse : ApiResponse<object>
{
    public ErrorResponse(string message = "An error occurred", string? errorCode = null, object? errors = null)
    {
        Success = false;
        Message = message;
        ErrorCode = errorCode;
        Errors = errors;
    }
}