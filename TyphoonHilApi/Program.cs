using System;
using System.ComponentModel.Design;
using System.Net.Sockets;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json.Linq;

namespace ZeroMQExample
{
    class Program
    {
        private static int SchematicApiPort;
        private const int _startPort = 50000;
        private const int _endPort = 50100;
        private const int _request_retries = 30;
        private const int _timeout = 1000;

        private static void Discover(int startPort=_startPort, int endPort=_endPort, int req_retries=_request_retries, int timeout=_timeout)
        {
            var offset = endPort - startPort + 1;
            using (var socket = new SubscriberSocket())
            {
                for (int i = 0; i < offset; i++)
                {
                    var port = startPort + i;
                    socket.Connect($"tcp://localhost:{port}");
                }

                socket.Subscribe("");
                socket.Options.Linger = TimeSpan.FromMilliseconds(0);
                
                socket.Poll(PollEvents.PollIn, TimeSpan.FromMilliseconds(timeout));
                using (var poller = new NetMQPoller { socket })
                {
                    while(req_retries != 0)
                    {
                        if(socket.Poll(PollEvents.PollIn, TimeSpan.FromMilliseconds(timeout)) == PollEvents.PollIn)
                        {
                            var res = socket.ReceiveFrameString();
                            if (res == null) break;
                            var parsedRes = JObject.Parse(res);
                            JArray result = parsedRes["result"]!.Value<JArray>()!;
                            string header = result[0].ToString();
                            if (header != "typhoon-service-registry") continue;

                            JObject apiPorts = result[2].ToObject<JObject>()!;
                            SchematicApiPort = apiPorts["sch_api"]!["server_rep_port"]!.Value<int>();
                            int hilPort = apiPorts["hil_api"]!["server_rep_port"]!.Value<int>();
                            int scadaPort = apiPorts["scada_api"]!["server_rep_port"]!.Value<int>();
                            int pvGenPort = apiPorts["pv_gen_api"]!["server_rep_port"]!.Value<int>();
                            int fwPort = apiPorts["fw_api"]!["server_rep_port"]!.Value<int>();
                            return;
                        }
                        else
                        {
                            req_retries--;
                        }
                    }
                }

                throw new Exception();

            }
        }
        static void Main(string[] args)
        {
            Discover();
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
    }
}
