namespace TouristarBackend.Exceptions;

public class UserExistsException : Exception
{
    public UserExistsException() : base(message: "A user with the provided credentials already exists.")
    {
    }

    public UserExistsException(string message) : base(message)
    {
    }

    public UserExistsException(string message, Exception inner) : base(message, inner)
    {
    }
}