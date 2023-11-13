namespace AHeat.Application.Exceptions;
public class DiscoverException : Exception
{
    public DiscoverException(string message)
        : base(message)
    {
    }

    public DiscoverException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
