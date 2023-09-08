using System.Runtime.Serialization;

namespace TyphoonHil.Exceptions;

public class ScadaAPIException : Exception
{
    public ScadaAPIException()
    {
    }

    public ScadaAPIException(string? message) : base(message)
    {
    }

    public ScadaAPIException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ScadaAPIException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}