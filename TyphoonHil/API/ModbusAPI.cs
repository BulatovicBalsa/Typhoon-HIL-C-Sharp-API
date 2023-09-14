using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TyphoonHil.Communication;
using TyphoonHil.Exceptions;

namespace TyphoonHil.API
{
    public class ModbusClient 
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public int UintId { get; set; }
        public double Timeout { get; set; }
        public bool Debug { get; set; }
        public bool AutoOpen { get; set; }
        public bool AutoClose { get; set; }
        public Socket? Sock { get; set; } = null;
        public string Version { get; private set; } = "0.2.0";

        public bool IsOpen => Sock?.Connected ?? false;

        public ModbusClient(string host= "localhost", int port= 502, int unitId= 1, double timeout= 30.0, bool debug= false, bool autoOpen= true, bool autoClose= false)
        {
            Host = host;
            Port = port;
            UintId = unitId;
            Timeout = timeout;
            Debug = debug;
            AutoOpen = autoOpen;
            AutoClose = autoClose;
        }

        public bool Open()
        {
            if (IsOpen) return true;
            return false;
        }


    }
}
