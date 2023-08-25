using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TyphoonHilApi.Communication
{
    internal class AbsractAPI : ICommunication
    {
        private int _schematicApiPort;
        private int _hilApiPort;
        private int _scadaApiPort;
        public static JObject GenerateMessageBase()
        {
            JObject parameters = new JObject();
            JObject message = new JObject();
            message.Add("api", "1.0");
            message.Add("jsonrpc", "2.0");
            message.Add("id", Guid.NewGuid().ToString());

            return message;
        }

        public void Request(string method, JObject parameters)
        {
            var message = CreateMessage(method, parameters);

            using (var reqSocket = new RequestSocket())
            {
                // Connect to the server
                reqSocket.Connect($"tcp://localhost:{SchematicApiPort}");

                // Request message
                JObject parameters = new JObject();
                parameters.Add("filename", "C:\\ex.tse");

                JObject message = new JObject();
                message.Add("api", "1.24.0");
                message.Add("jsonrpc", "2.0");
                message.Add("method", "compile");
                message.Add("params", new JObject());
                //message.Add("params", parameters);
                message.Add("id", 1);

                reqSocket.SendFrame(message.ToString());

                var answer = reqSocket.ReceiveFrameString();
                Console.WriteLine(answer);
                reqSocket.Close();
            }
        }

        private static JObject CreateMessage(string method, JObject parameters)
        {
            var message = GenerateMessageBase();
            message.Add("method", method);
            message.Add("parameters", parameters);
            return message;
        }

        public JObject Response()
        {
            throw new NotImplementedException();
        }
    }
}
