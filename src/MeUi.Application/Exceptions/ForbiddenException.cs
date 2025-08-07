using System.Net;

namespace MeUi.Application.Exceptions;

/// <summary>
/// Exception thrown when user is authenticated but lacks permission
/// </summary>
public class ForbiddenException : ApplicationException
{
    public ForbiddenException(string message = "Forbidden access")
        : base(message, (int)HttpStatusCode.Forbidden) { }
}