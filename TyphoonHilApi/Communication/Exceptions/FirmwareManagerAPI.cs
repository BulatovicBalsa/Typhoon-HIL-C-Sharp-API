using Newtonsoft.Json.Linq;
using TyphoonHilApi.Communication.APIs;

namespace TyphoonHilApi.Communication.Exceptions;

public class FirmwareManagerAPI : AbsractAPI
{
    public override int ProperPort => Ports.FwApiPort;

    protected override JObject HandleRequest(string method, JObject parameters)
    {
        var res = Request(method, parameters);
        if (!res.ContainsKey("error")) return res;
        var msg = (string)res["error"]!["message"]!;
        throw new FirmwareManagerAPIException(msg);
    }

    public JObject GetHilInfo()
    {
        return (JObject)HandleRequest("get_hil_info")["result"]!;
    }
}