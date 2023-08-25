using Newtonsoft.Json.Linq;

namespace TyphoonHilApi.Communication.APIs
{
    internal class SchematicAPI : AbsractAPI
    {
        public override int ProperPort => _schematicApiPort;

        public JObject Load(string filename)
        {
            return Request("load", new JObject { "filename", filename });
        }

        public JObject Compile()
        {
            return Request("compile", new());
        }
    }
}
