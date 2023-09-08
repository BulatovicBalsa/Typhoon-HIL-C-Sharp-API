namespace TyphoonHil.Exceptions;

internal class ConfigurationManagerAPIException : Exception
{
    public ConfigurationManagerAPIException()
    {
    }

    public ConfigurationManagerAPIException(string message) : base(message)
    {
    }
}