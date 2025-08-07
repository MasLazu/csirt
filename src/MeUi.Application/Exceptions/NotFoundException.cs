using System.Net;

namespace MeUi.Application.Exceptions;

/// <summary>
/// Exception thrown when a requested resource is not found
/// </summary>
public class NotFoundException : ApplicationException
{
    public NotFoundException(string entityName)
        : base($"{entityName} not found", (int)HttpStatusCode.NotFound) { }

    public NotFoundException(string entityName, object key)
        : base($"{entityName} with key '{key}' not found", (int)HttpStatusCode.NotFound) { }
}