using Newtonsoft.Json.Linq;
using TyphoonHilApi.Communication.Exceptions;

namespace TyphoonHilApi.Communication.APIs
{
    public class PvModelType
    {
        public const string DETAILED = "Detailed";
        public const string EN50530 = "EN50530 Compatible";
        public const string NORMALIZED_IV = "Normalized IV";
    }

    public class PvResponse
    {
        public bool Status { get; set; }
        public string? Message { get; set; }

        public PvResponse(JArray jArray) { Status = (bool)jArray[0]; Message = (string?)jArray[1]??null; }

        public override string ToString()
        {
            return $"{Status}, {Message}";
        }
    }

    public class PvGeneratorAPI : AbsractAPI
    {
        public readonly List<string> EN50530_PV_TYPES = new() { "cSi", "Thin film", "User defined" };
        public readonly List<string> DETAILED_PV_TYPE = new() { "cSi", "Amorphous Si" };

        public override int ProperPort => Ports.PvGenApiPort;

        protected override JObject HandleRequest(string method, JObject parameters)
        {
            var res = Request(method, parameters);
            if (res.ContainsKey("error"))
            {
                var msg = (string)res["error"]!["message"]!;
                throw new PvGeneratorAPIException(msg);
            }

            return res;
        }

        public PvResponse GeneratePvSettingsFile(string modelType, string fileName, JObject parameters)
        {
            var requestParameters = new JObject()
            {
                { "modelType", modelType },
                { "fileName", fileName },
                { "parameters", parameters },
            };
            PvResponse res = new((JArray)HandleRequest("generate_pv_settings_file", requestParameters)["result"]!);
            Console.WriteLine(res);
            return res;
        }
    }
}
