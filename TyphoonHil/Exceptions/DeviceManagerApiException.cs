namespace TyphoonHil.Exceptions;

public class DeviceManagerApiException : Exception
{
    public DeviceManagerApiException()
    {
    }

    public DeviceManagerApiException(string message) : base(message)
    {
    }
}