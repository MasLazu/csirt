using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using MeUi.Application.Exceptions;
using MeUi.Api.Models;

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

        ErrorResponse response = exception switch
        {
            FastEndpoints.ValidationFailureException ex => new ErrorResponse(
                message: "One or more validation errors occurred.",
                errorCode: "VALIDATION_ERROR",
                errors: ex.Failures?
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray())),
            Application.Exceptions.ApplicationException ex => new ErrorResponse(
                message: ex.Message,
                errorCode: ex.ErrorCode,
                errors: ex.Errors),
            _ => new ErrorResponse(message: "An unexpected error occurred")
        };

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        await httpContext.Response.WriteAsync(
            JsonSerializer.Serialize(response, jsonOptions),
            cancellationToken);

        return true;
    }
}