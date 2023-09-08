using Newtonsoft.Json.Linq;
using TyphoonHilApi.Communication.Exceptions;

namespace TyphoonHilApi.API;

public class PvModelType
{
    public const string Detailed = "Detailed";
    public const string En50530 = "EN50530 Compatible";
    public const string NormalizedIv = "Normalized IV";
}

public class PvResponse
{
    public PvResponse(JArray jArray)
    {
        Status = (bool)jArray[0];
        Message = (string?)jArray[1] ?? null;
    }

    public bool Status { get; set; }
    public string? Message { get; set; }

    public override string ToString()
    {
        return $"{Status}, {Message}";
    }
}

public class PvGeneratorAPI : AbstractAPI
{
    public readonly List<string> DetailedPvType = new() { "cSi", "Amorphous Si" };
    public readonly List<string> En50530PvTypes = new() { "cSi", "Thin film", "User defined" };

    public override int ProperPort => Ports.PvGenApiPort;

    protected override JObject HandleRequest(string method, JObject parameters)
    {
        var res = Request(method, parameters);
        if (!res.ContainsKey("error")) return res;
        var msg = (string)res["error"]!["message"]!;
        throw new PvGeneratorAPIException(msg);
    }

    public PvResponse GeneratePvSettingsFile(string modelType, string fileName, JObject parameters)
    {
        var requestParameters = new JObject
        {
            { "modelType", modelType },
            { "fileName", fileName },
            { "parameters", parameters }
        };
        PvResponse res = new((JArray)HandleRequest("generate_pv_settings_file", requestParameters)["result"]!);
        Console.WriteLine(res);
        return res;
    }
}