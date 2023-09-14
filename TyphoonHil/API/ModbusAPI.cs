using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TyphoonHil.Communication;
using TyphoonHil.Exceptions;

namespace TyphoonHil.API
{
    public class ModbusAPI : AbstractAPI
    {
        public ModbusAPI()
        {
        }

        internal ModbusAPI(ICommunication communication) : base(communication)
        {
        }

        protected override int ProperPort => Ports.HilApiPort;

        protected override JObject HandleRequest(string method, JObject parameters)
        {
            {
                var res = Request(method, parameters);
                if (!res.ContainsKey("error")) return res;
                var msg = (string)res["error"]!["message"]!;
                throw new HilAPIException(msg);
            }
        }
    }
}
