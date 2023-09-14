namespace TyphoonHil.Exceptions;

public class ModbusApiException : Exception
{
    public ModbusApiException()
    {
    }

    public ModbusApiException(string message) : base(message)
    {
    }
}