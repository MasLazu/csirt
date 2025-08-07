using System.Net;

namespace MeUi.Application.Exceptions;

/// <summary>
/// Exception thrown when a conflict occurs (e.g., duplicate resource)
/// </summary>
public class BadRequestException : ApplicationException
{
    public BadRequestException(string message = "Bad Request")
        : base(message, (int)HttpStatusCode.BadRequest) { }
}