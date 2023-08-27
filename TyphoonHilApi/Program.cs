using Newtonsoft.Json.Linq;
using System;
using TyphoonHilApi.Communication.APIs;

namespace ZeroMQExample
{
    class Program
    {
        static void Main(string[] args)
        {
            SchematicAPI mdl = new SchematicAPI();

            string filePath = Path.Combine(
                Path.GetDirectoryName(Path.GetFullPath(Environment.GetCommandLineArgs()[0]))!,
                "create_library_model_lib",
                "example_library.tlib"
            );

            string libName = "Example Library";

            mdl.CreateLibraryModel(
                libName,
                filePath
            );

            //
            // Create basic components, connect them and add them to the library
            //
            JObject r = mdl.CreateComponent("core/Resistor", name: "R1");
            JObject c = mdl.CreateComponent("core/Capacitor", name: "C1");

            JObject con = mdl.CreateConnection(mdl.Term(c, "n_node"),
                                              mdl.Term(r, "p_node"));

            JObject con1 = mdl.CreateConnection(mdl.Term(c, "p_node"),
                                               mdl.Term(r, "n_node"));

            //
            // Save the library, load it and try saving the loaded library
            //
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            }
            mdl.SaveAs(filePath);
            mdl.Load(filePath);
            mdl.Save();
            Console.WriteLine(filePath);

        }

        private static void Test3()
        {
            SchematicAPI mdl = new SchematicAPI();
            mdl.CreateNewModel();

            // Create comment with some text
            JObject comment1 = mdl.CreateComment("This is a comment");

            //
            // Create comment with text, custom name, and specified position
            // in the scheme.
            //
            JObject comment2 = mdl.CreateComment("This is a comment 2", name: "Comment 2", position: new(100, 200));

            Console.WriteLine("Comment is {0}.", comment2);

            mdl.CloseModel();
        }

        private static void Test2()
        {
            SchematicAPI mdl = new SchematicAPI();
            mdl.CreateNewModel();

            //
            // Library file is located in directory 'custom_lib' one level above this
            // example file.
            //

            string directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!;
            string libPath = System.IO.Path.Combine(directory, "custom_lib");

            // Get all current library paths and remove them
            List<string> oldPaths = mdl.GetLibraryPaths();
            oldPaths.ForEach(x => mdl.RemoveLibraryPath(x));

            // Add library path and reload library to be able to use the added library.
            mdl.AddLibraryPath(libPath);
            mdl.ReloadLibraries();

            // Create components from loaded libraries.
            var comp = mdl.CreateComponent("my_lib/CustomComponent");
            Console.WriteLine($"Component is '{comp}'.");

            var comp2 = mdl.CreateComponent("archived_user_lib/CustomComponent1");
            Console.WriteLine($"Second component (from archived library) is '{comp2}'.");

            // Remove library from the path.
            mdl.RemoveLibraryPath(libPath);

            // Add again the previous library paths
            foreach (string path in oldPaths)
            {
                mdl.AddLibraryPath(path);
            }

            mdl.CloseModel();
        }

        private static void Test1()
        {
            string path = "C:\\Users\\Dell\\source\\repos\\TyphoonHilApi\\TestData\\";

            // Create SchematicAPI object
            SchematicAPI model = new();

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

            // Create Current Measurement component
            var iMeas = model.CreateComponent("core/Current Measurement", name: "I", position: new(x0 - 100, y0 - 100));

            // Create Ground component
            var gnd = model.CreateComponent("core/Ground", name: "gnd", position: new(x0 - 300, y0 + 200));

            // Create Inductor component
            var ind = model.CreateComponent("core/Inductor", name: "L", position: new(x0, y0), rotation: Rotation.Right);

            // Create Voltage Measurement component
            var vMeas = model.CreateComponent("core/Voltage Measurement", name: "V", position: new(x0 + 200, y0), rotation: Rotation.Right);

            // Create RC Load Subsystem component
            var rcLoad = model.CreateComponent("core/Empty Subsystem", name: "RC Load", position: new(x0 + 100, y0));

            // Create port in Subsystem
            var p1 = model.CreatePort(name: "P1", parent: rcLoad, terminalPosition: new TerminalPosition(TerminalPosition.Top, TerminalPosition.Auto), rotation: Rotation.Right, position: new(x0, y0 - 200));

            // Create port in Subsystem
            var p2 = model.CreatePort(name: "P2", parent: rcLoad, terminalPosition: new TerminalPosition(TerminalPosition.Bottom, TerminalPosition.Auto), rotation: Rotation.Left, position: new(x0, y0 + 200));

            // Create Resistor component
            var r = model.CreateComponent("core/Resistor", parent: rcLoad, name: "R", position: new(x0, y0 - 50), rotation: Rotation.Right);

            // Create Capacitor component
            var c = model.CreateComponent("core/Capacitor", parent: rcLoad, name: "C", position: new(x0, y0 + 50), rotation: Rotation.Right);

            // Create necessary junctions
            var junction1 = model.CreateJunction(name: "J1", position: new(x0 - 300, y0 + 100));

            var junction2 = model.CreateJunction(name: "J2", position: new(x0, y0 - 100));

            var junction3 = model.CreateJunction(name: "J3", position: new(x0, y0 + 100));

            var junction4 = model.CreateJunction(name: "J4", position: new(x0 + 100, y0 - 100));

            var junction5 = model.CreateJunction(name: "J5", position: new(x0 + 100, y0 + 100));

            // Connect all the components
            Console.WriteLine("Connecting components...");
            model.CreateConnection(model.Term(vIn, "p_node"), model.Term(rIn, "p_node"));
            model.CreateConnection(model.Term(vIn, "n_node"), junction1);
            model.CreateConnection(model.Term(gnd, "node"), junction1);
            model.CreateConnection(model.Term(rIn, "n_node"), model.Term(iMeas, "p_node"));
            model.CreateConnection(model.Term(iMeas, "n_node"), junction2);
            model.CreateConnection(junction2, model.Term(ind, "p_node"));
            model.CreateConnection(model.Term(ind, "n_node"), junction3);
            model.CreateConnection(junction1, junction3);
            model.CreateConnection(junction2, junction4);
            model.CreateConnection(junction3, junction5);
            model.CreateConnection(model.Term(rcLoad, "P1"), junction4);
            model.CreateConnection(junction5, model.Term(rcLoad, "P2"));
            model.CreateConnection(junction4, model.Term(vMeas, "p_node"));
            model.CreateConnection(model.Term(vMeas, "n_node"), junction5);
            model.CreateConnection(p1, model.Term(r, "p_node"));
            model.CreateConnection(model.Term(r, "n_node"), model.Term(c, "p_node"));
            model.CreateConnection(model.Term(c, "n_node"), p2);

            // Set component parameters
            Console.WriteLine("Setting component properties...");
            model.SetPropertyValue(model.Prop(rIn, "resistance"), rInValue);
            model.SetPropertyValue(model.Prop(ind, "inductance"), lValue);
            model.SetPropertyValue(model.Prop(r, "resistance"), rValue);
            model.SetPropertyValue(model.Prop(c, "capacitance"), cValue);


            // Save the model
            string fileName = path + "RLC_example.tse";
            Console.WriteLine($"Saving model to '{fileName}'...");
            model.SaveAs(fileName);

            // Compile model
            if (model.Compile())
                Console.WriteLine("Model successfully compiled.");
            else
                Console.WriteLine("Model failed to compile");

            // Close the model
            model.CloseModel();
        }
    }
}
