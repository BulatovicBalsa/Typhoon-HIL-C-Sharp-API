using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TyphoonHilApi.Communication.Exceptions;

namespace TyphoonHilApi.Communication.APIs
{
    internal class HilAPI : AbsractAPI
    {
        public override int ProperPort => Ports.HilApiPort;
        public HilAPI() { }

        public HilAPI(ICommunication communication):base(communication) { }

        public bool LoadModel(string file = "", bool offlineMode = false, bool vhilDevice = false)
        {
            var parameters = new JObject()
            {
                { "file", file },
                { "offline_mode", offlineMode },
                { "vhil_device", vhilDevice },
            };

            return (bool)HandleRequest("load_model", parameters)["result"]!;
        }

        protected override JObject HandleRequest(string method, JObject parameters)
        {
            {
                var res = Request(method, parameters);
                if (res.ContainsKey("error"))
                {
                    var msg = (string)res["error"]!["message"]!;
                    throw new HilAPIException(msg);
                }

                return res;
            }
        }
    }
}
