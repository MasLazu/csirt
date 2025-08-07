namespace MeUi.Application.Exceptions;

/// <summary>
/// Base class for all application exceptions
/// </summary>
public abstract class ApplicationException : Exception
{
    public int StatusCode { get; set; }
    public IEnumerable<string>? Errors { get; set; }
    public string ErrorCode { get; set; }

    protected ApplicationException(string message, int statusCode, IEnumerable<string>? errors = null) : base(message)
    {
        ErrorCode = ToScreamingSnakeCase(GetType().Name.Replace("Exception", string.Empty));
        StatusCode = statusCode;
        Errors = errors;
    }

    protected ApplicationException(string message, int statusCode, string error) : base(message)
    {
        StatusCode = statusCode;
        Errors = new List<string> { error };
        ErrorCode = ToScreamingSnakeCase(GetType().Name.Replace("Exception", string.Empty));
    }

    private static string ToScreamingSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        var sb = new System.Text.StringBuilder();
        bool prevUpper = true;

        foreach (char c in input)
        {
            if (char.IsUpper(c))
            {
                if (!prevUpper)
                {
                    sb.Append('_');
                }
                sb.Append(char.ToUpper(c));
                prevUpper = true;
            }
            else
            {
                sb.Append(char.ToUpper(c));
                prevUpper = false;
            }
        }

        return sb.ToString();
    }
}