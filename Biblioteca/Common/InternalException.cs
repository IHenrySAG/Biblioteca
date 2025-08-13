namespace Biblioteca.Common;

public class InternalException:Exception
{
    public InternalException(string message) : base(message)
    {
    }
    public InternalException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
