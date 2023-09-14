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
        public string Host { get; set; }
        public int Port { get; set; }
        public int UintId { get; set; }

        public ModbusAPI(string host= "localhost", int port= 502, int unitId= 1, double timeout= 30.0, bool debug= false, bool autoOpen= true, bool autoClose= false)
        {

        }

        internal ModbusAPI(ICommunication communication) : base(communication)
        {
        }

        protected override int ProperPort => Ports.ModbusApiPort;

        protected override JObject HandleRequest(string method, JObject parameters)
        {
            {
                var res = Request(method, parameters);
                if (!res.ContainsKey("error")) return res;
                var msg = (string)res["error"]!["message"]!;
                throw new ModbusApiException(msg);
            }
        }


    }
}
