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
            var msg = new JObject();
            var scApi = new SchematicAPI();

            //msg = scApi.Load("C:\\Users\\Dell\\Documents\\balsa\\bla.tse");
            Console.WriteLine(msg.ToString());
            //Console.WriteLine("===========================");
            //msg = scApi.Compile();
            //Console.WriteLine(msg.ToString());
            //msg = scApi.SaveAs("C:\\Users\\Dell\\Documents\\balsa\\blabla.tse");
            msg = scApi.GetLibraryPaths();
            Console.WriteLine(msg.ToString());
        }
    }
}
