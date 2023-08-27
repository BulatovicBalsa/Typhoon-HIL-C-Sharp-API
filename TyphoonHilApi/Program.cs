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

            var iMeas = model.CreateComponent("core/Current Measurement", name: "I", position: new(x0 - 100, y0 - 100));

            var gnd = model.CreateComponent("core/Ground", name: "gnd", position: new(x0 - 300, y0 + 200));

            var ind = model.CreateComponent("core/Inductor", name: "L", position: new(x0, y0), rotation: Rotation.Right);

            var vMeas = model.CreateComponent("core/Voltage Measurement", name: "V", position: new(x0 + 200, y0), rotation: Rotation.Right);

            var rcLoad = model.CreateComponent("core/Empty Subsystem", name: "RC Load", position: new(x0 + 100, y0));

            var p1 = model.CreatePort(name: "P1", parent: rcLoad, terminalPosition: new TerminalPosition(TerminalPosition.Top, TerminalPosition.Auto), rotation: Rotation.Right, position: new(x0, y0 - 200));
            // Continue creating other components...
            var p2 = model.CreatePort(name: "P2", parent: rcLoad, terminalPosition: new TerminalPosition(TerminalPosition.Bottom, TerminalPosition.Auto), rotation: Rotation.Left, position: new(x0, y0 + 200));

            var r = model.CreateComponent("core/Resistor", parent: rcLoad, name: "R", position: new(x0, y0 - 50), rotation: Rotation.Right);

            var c = model.CreateComponent("core/Capacitor", parent: rcLoad, name: "C", position: new(x0, y0 + 50), rotation: Rotation.Right);

            var junction1 = model.CreateJunction(name: "J1", position: new(x0 - 300, y0 + 100));

            var junction2 = model.CreateJunction(name: "J2", position: new(x0, y0 - 100));

            var junction3 = model.CreateJunction(name: "J3", position: new(x0, y0 + 100));

            var junction4 = model.CreateJunction(name: "J4", position: new(x0 + 100, y0 - 100));

            var junction5 = model.CreateJunction(name: "J5", position: new(x0 + 100, y0 + 100));

            model.CreateConnection(
                model.Term(vIn, "p_node"),
                model.Term(rIn, "p_node")
            );

            model.CreateConnection(
                model.Term(vIn, "n_node"),
                junction1
            );

            model.CreateConnection(
                model.Term(gnd, "node"),
                junction1
            );

            model.CreateConnection(
                model.Term(rIn, "n_node"),
                model.Term(iMeas, "p_node")
            );

            model.CreateConnection(
                model.Term(iMeas, "n_node"),
                junction2
            );

            model.CreateConnection(junction2, model.Term(ind, "p_node"));

            model.CreateConnection(
                model.Term(ind, "n_node"),
                junction3
            );

            model.CreateConnection(junction1, junction3);
            model.CreateConnection(junction2, junction4);
            model.CreateConnection(junction3, junction5);

            model.CreateConnection(
                model.Term(rcLoad, "P1"),
                junction4
            );

            model.CreateConnection(junction5, model.Term(rcLoad, "P2"));

            model.CreateConnection(
                junction4,
                model.Term(vMeas, "p_node")
            );

            model.CreateConnection(
                model.Term(vMeas, "n_node"),
                junction5
            );

            model.CreateConnection(p1, model.Term(r, "p_node"));
            model.CreateConnection(model.Term(r, "n_node"), model.Term(c, "p_node"));
            model.CreateConnection(model.Term(c, "n_node"), p2);

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
