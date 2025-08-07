using System.Net;

namespace MeUi.Application.Exceptions;

/// <summary>
/// Exception thrown when validation fails
/// </summary>
public class ValidationException : ApplicationException
{
    public ValidationException(string message)
        : base(message, (int)HttpStatusCode.BadRequest) { }

    public ValidationException(IEnumerable<string> errors)
        : base("One or more validation errors occurred", (int)HttpStatusCode.BadRequest, errors) { }

    public ValidationException(string field, string error)
        : base($"Validation failed for {field}", (int)HttpStatusCode.BadRequest, new[] { error }) { }
}