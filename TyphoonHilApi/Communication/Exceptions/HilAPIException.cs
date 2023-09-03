namespace TyphoonHilApi.Communication.Exceptions;

public class HilAPIException : Exception
{
    public HilAPIException()
    {
    }

    public HilAPIException(string message) : base(message)
    {
    }
}