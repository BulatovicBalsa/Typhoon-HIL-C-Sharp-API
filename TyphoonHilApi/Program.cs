using System;
using System.ComponentModel.Design;
using System.Net.Sockets;
using System.Text;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json.Linq;
using TyphoonHilApi.Communication.APIs;

namespace ZeroMQExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var scApi = new SchematicAPI();
            var msg = scApi.Load("C:\\ex.tse");
            Console.WriteLine(msg.ToString());
            Console.WriteLine("===========================");
            msg = scApi.Compile();
            Console.WriteLine(msg.ToString());
        }
    }
}
