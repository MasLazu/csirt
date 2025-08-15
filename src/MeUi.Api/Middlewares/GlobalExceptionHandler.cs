using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using MeUi.Application.Exceptions;
using MeUi.Api.Models;
using Newtonsoft.Json;

namespace MeUi.Api.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        httpContext.Response.ContentType = "application/json";

        (ErrorResponse response, int statusCode) = exception switch
        {
            JsonReaderException ex => (new ErrorResponse(
                message: "Invalid JSON format.",
                errorCode: "INVALID_JSON",
                errors: new[] { ex.Message }), (int)HttpStatusCode.BadRequest),
            System.Text.Json.JsonException ex => (new ErrorResponse(
                message: "Invalid JSON format.",
                errorCode: "INVALID_JSON",
                errors: new[] { ex.Message }), (int)HttpStatusCode.BadRequest),
            FluentValidation.ValidationException ex => (new ErrorResponse(
                message: "One or more validation errors occurred.",
                errorCode: "VALIDATION_ERROR",
                errors: ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray())), (int)HttpStatusCode.BadRequest),
            FastEndpoints.ValidationFailureException ex => (new ErrorResponse(
                message: "One or more validation errors occurred.",
                errorCode: "VALIDATION_ERROR",
                errors: ex.Failures?
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray())), (int)HttpStatusCode.BadRequest),
            Application.Exceptions.ApplicationException ex => (new ErrorResponse(
                message: ex.Message,
                errorCode: ex.ErrorCode,
                errors: ex.Errors), ex.StatusCode),
            _ => (new ErrorResponse(message: "An unexpected error occurred"), (int)HttpStatusCode.InternalServerError)
        };

        httpContext.Response.StatusCode = statusCode;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        await httpContext.Response.WriteAsync(
            System.Text.Json.JsonSerializer.Serialize(response, jsonOptions),
            cancellationToken);

        return true;
    }
}