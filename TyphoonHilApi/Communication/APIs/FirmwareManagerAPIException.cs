namespace TyphoonHilApi.Communication.APIs;

public class FirmwareManagerAPIException : Exception
{
    public FirmwareManagerAPIException(string message) : base(message)
    {
    }

    public FirmwareManagerAPIException() : base()
    {
    }
}