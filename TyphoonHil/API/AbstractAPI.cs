using Newtonsoft.Json.Linq;
using TyphoonHil.Communication;

namespace TyphoonHil.API;

public abstract class AbstractAPI
{
    private readonly ICommunication _communication;

    protected AbstractAPI(ICommunication communication)
    {
        _communication = communication;
        Ports = _communication.Discover();
    }

    protected AbstractAPI()
    {
        _communication = new NetMQCommunication();
        Ports = _communication.Discover();
    }

    public abstract int ProperPort { get; }
    public PortsDto Ports { get; set; }

    public JObject Request(string method, JObject parameters)
    {
        return _communication.Request(method, parameters, ProperPort);
    }

    protected abstract JObject HandleRequest(string method, JObject parameters);

    protected JObject HandleRequest(string method)
    {
        return HandleRequest(method, new JObject());
    }
}