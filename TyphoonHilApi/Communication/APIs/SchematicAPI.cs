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

        public JObject CreateNewModel()
        {
            return Request("create_new_model", new());
        }

        public JObject CloseModel()
        {
            return Request("close_model", new());
        }

        public JObject SaveAs(string filename)
        {
            return Request("save_as", new JObject() { { "filename", filename } });
        }

        public JObject GetLibraryPaths() 
        {
            return Request("get_library_paths", new());
        }
    }
}
