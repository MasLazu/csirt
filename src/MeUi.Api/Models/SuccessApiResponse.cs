using System.Text.Json.Serialization;

namespace MeUi.Api.Models;

public class SuccessApiResponse<T> : ApiResponse<T>
{
    public SuccessApiResponse(T? data, string message = "Operation successful")
    {
        Success = true;
        Data = data;
        Message = message;
        ErrorCode = null;
        Errors = null;
    }
}