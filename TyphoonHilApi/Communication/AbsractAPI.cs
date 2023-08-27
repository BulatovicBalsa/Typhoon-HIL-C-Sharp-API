using Newtonsoft.Json.Linq;
using TyphoonHilApi.Communication.Exceptions;

namespace TyphoonHilApi.Communication
{
    internal abstract class AbsractAPI
    {
        public abstract int ProperPort { get; }
        private ICommunication _communication { get; set; }
        public PortsDto Ports { get; set; }

        public AbsractAPI(ICommunication communication)
        {
            this._communication = communication;
            Ports = _communication.Discover();
        }

        public AbsractAPI()
        {
            this._communication = new NetMQCommunication();
            Ports = _communication.Discover();
        }

        public JObject Request(string method, JObject parameters)
        {
            return _communication.Request(method, parameters, ProperPort);
        }

        protected JObject HandleRequest(string method, JObject parameters)
        {
            var res = Request(method, parameters);
            if (res.ContainsKey("error"))
            {
                var msg = (string)res["error"]!["message"]!;
                throw new SchematicAPIException(msg);
            }

            return res;
        }
    }
}
