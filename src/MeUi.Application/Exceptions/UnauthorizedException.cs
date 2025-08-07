using System.Net;

namespace MeUi.Application.Exceptions;

/// <summary>
/// Exception thrown when user is not authenticated
/// </summary>
public class UnauthorizedException : ApplicationException
{
    public UnauthorizedException(string message = "Unauthorized access")
        : base(message, (int)HttpStatusCode.Unauthorized) { }
}