using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TyphoonHilApi.Communication.APIs
{
    internal class ConfigurationManagerAPI : AbsractAPI
    {
        public override int ProperPort => Ports.ConfigurationManagerApiPort;

        public ConfigurationManagerAPI() { }

        public ConfigurationManagerAPI(ICommunication communication) : base(communication) { }

        public JObject LoadProject(string projectPath)
        {
            var parameters = new JObject()
            {
                { "project_path", projectPath } 
            };

            return (JObject)HandleRequest("load_project", parameters)["result"]!;
        }
    }
}
