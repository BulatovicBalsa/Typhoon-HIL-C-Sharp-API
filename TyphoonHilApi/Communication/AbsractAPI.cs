using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json.Linq;

namespace TyphoonHilApi.Communication
{
    internal abstract class AbsractAPI : ICommunication
    {
        protected int _schematicApiPort;
        protected int _hilApiPort;
        protected int _scadaApiPort;
        protected int _pvGenApiPort;
        protected int _fwApiPort;

        public abstract int ProperPort { get; }

        public AbsractAPI()
        {
            Discover();
        }

        private void Discover(int startPort = 50000, int endPort = 50100, int req_retries = 30, int timeout = 1000)
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
                    while (req_retries != 0)
                    {
                        if (socket.Poll(PollEvents.PollIn, TimeSpan.FromMilliseconds(timeout)) == PollEvents.PollIn)
                        {
                            var res = socket.ReceiveFrameString();
                            if (res == null) break;
                            var parsedRes = JObject.Parse(res);
                            JArray result = parsedRes["result"]!.Value<JArray>()!;
                            string header = result[0].ToString();
                            if (header != "typhoon-service-registry") continue;

                            JObject apiPorts = result[2].ToObject<JObject>()!;
                            _schematicApiPort = apiPorts["sch_api"]!["server_rep_port"]!.Value<int>();
                            _hilApiPort = apiPorts["hil_api"]!["server_rep_port"]!.Value<int>();
                            _scadaApiPort = apiPorts["scada_api"]!["server_rep_port"]!.Value<int>();
                            _pvGenApiPort = apiPorts["pv_gen_api"]!["server_rep_port"]!.Value<int>();
                            _fwApiPort = apiPorts["fw_api"]!["server_rep_port"]!.Value<int>();
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
        public static JObject GenerateMessageBase()
        {
            JObject message = new()
            {
                { "api", "1.0" },
                { "jsonrpc", "2.0" },
                { "id", Guid.NewGuid().ToString() }
            };

            return message;
        }

        public JObject Request(string method, JObject parameters)
        {
            var message = CreateMessage(method, parameters);

            using (var reqSocket = new RequestSocket())
            {
                // Connect to the server
                reqSocket.Connect($"tcp://localhost:{ProperPort}");
                reqSocket.SendFrame(message.ToString());

                var answer = reqSocket.ReceiveFrameString();
                reqSocket.Close();
                return JObject.Parse(answer);
            }
        }

        private static JObject CreateMessage(string method, JObject parameters)
        {
            var message = GenerateMessageBase();
            message.Add("method", method);
            message.Add("params", parameters);
            return message;
        }
    }
}
