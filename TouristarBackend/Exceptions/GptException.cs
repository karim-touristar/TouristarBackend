namespace TouristarBackend.Exceptions;

public class GptException : Exception
{
    public GptException()
    {
    }

    public GptException(string message) : base(message)
    {
    }

    public GptException(string message, Exception inner) : base(message, inner)
    {
    }
}