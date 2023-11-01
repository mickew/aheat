namespace AHeat.Application.Exceptions;
public class WebHookException : Exception
{
    public WebHookException(string message)
        : base(message)
    {
    }

    public WebHookException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
