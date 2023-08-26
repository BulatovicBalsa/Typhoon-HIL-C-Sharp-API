using Newtonsoft.Json.Linq;

namespace TyphoonHilApi.Communication.APIs
{
    internal class SchematicAPI : AbsractAPI
    {
        public SchematicAPI() { }
        public SchematicAPI(ICommunication communication):base(communication) { }
        
        public override int ProperPort => Ports.SchematicApiPort;

        public JObject Load(string filename)
        {
            return Request("load", new JObject() { { "filename", filename } });
        }

        public JObject Compile()
        {
            return Request("compile", new());
        }
    }
}
