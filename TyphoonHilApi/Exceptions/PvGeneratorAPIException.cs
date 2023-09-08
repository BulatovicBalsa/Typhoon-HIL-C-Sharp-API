using System.Runtime.Serialization;

namespace TyphoonHil.Exceptions;

[Serializable]
public class PvGeneratorAPIException : Exception
{
    public PvGeneratorAPIException()
    {
    }

    public PvGeneratorAPIException(string? message) : base(message)
    {
    }

    public PvGeneratorAPIException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected PvGeneratorAPIException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}