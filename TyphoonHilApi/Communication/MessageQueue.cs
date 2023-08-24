using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TyphoonHilApi.Communication
{
    internal class MessageQueue
    {
        private static MessageQueue? _instance;
        public static MessageQueue? Instance = _instance ??= new MessageQueue();

        public static void Send()
        {
            

            JObject parameters = new JObject();
            parameters.Add("filename", "abs_path_to_the_model");

            JObject message = new JObject();
            message.Add("api", "1.0");
            message.Add("jsonrpc", "2.0");
            message.Add("method", "load");
            message.Add("params", parameters);
            message.Add("id", 1);


        }

        public static void Receive() { }



    }
}
