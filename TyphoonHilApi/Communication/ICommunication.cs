using Newtonsoft.Json.Linq;

namespace TyphoonHilApi.Communication;

public interface ICommunication
{
    JObject Request(string operation, JObject parameters, int port);
    PortsDto Discover(int startPort = 50000, int endPort = 50100, int requestRetries = 30, int timeout = 1000);
}