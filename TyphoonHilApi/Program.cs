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
            string path = "C:\\Users\\Dell\\source\\repos\\TyphoonHilApi\\TestData\\";

            SchematicAPI model = new SchematicAPI();

            // Create new model
            model.CreateNewModel();

            // Starting coordinates
            int x0 = 8192;
            int y0 = 8192;

            // Component values
            double rInValue = 100.0;
            double lValue = 1e-5;
            double rValue = 0.1;
            double cValue = 5e-4;

            Console.WriteLine("Creating scheme items...");

            // Create Voltage Source component
            JObject vIn = model.CreateComponent("core/Voltage Source", name: "Vin", position: new(x0 - 300, y0), rotation: "right");

            // Create Resistor component
            JObject rIn = model.CreateComponent("core/Resistor", name: "Rin", position: new(x0 - 200, y0 - 100));

            // Continue creating other components...

            // Connect all the components
            Console.WriteLine("Connecting components...");
            //model.CreateConnection(model.Term(vIn, "p_node"), model.Term(rIn, "p_node"));
            // Continue creating connections...

            // Set component parameters
            Console.WriteLine("Setting component properties...");
            //model.SetPropertyValue(model.Prop(rIn, "resistance"), rInValue);
            // Continue setting properties...

            // Save the model
            string fileName = path + "RLC_example.tse";
            Console.WriteLine($"Saving model to '{fileName}'...");
            model.SaveAs(fileName);

            // Compile model
            if (model.Compile().ContainsKey("result"))
            {
                Console.WriteLine("Model successfully compiled.");
            }
            else
            {
                Console.WriteLine("Model failed to compile");
            }

            // Close the model
            model.CloseModel();

        }
    }
}
