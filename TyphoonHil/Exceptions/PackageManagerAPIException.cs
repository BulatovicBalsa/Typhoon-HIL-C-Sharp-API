namespace TyphoonHil.Exceptions;

public class PackageManagerAPIException : Exception
{
    public PackageManagerAPIException()
    {
    }

    public PackageManagerAPIException(string message) : base(message)
    {
    }
}