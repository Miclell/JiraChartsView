namespace Core.Exceptions;

public class JiraApiException : Exception
{
    public int ErrorCode { get; }

    public JiraApiException(string message) : base(message) { }

    public JiraApiException(string message, int errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
}