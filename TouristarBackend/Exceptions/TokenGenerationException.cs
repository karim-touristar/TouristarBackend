namespace TouristarBackend.Exceptions;

public class TokenGenerationException : Exception
{
    public TokenGenerationException() : base(message: "There was an issue generating tokens.")
    {
    }

    public TokenGenerationException(string message) : base(message)
    {
    }

    public TokenGenerationException(string message, Exception inner) : base(message, inner)
    {
    }
}