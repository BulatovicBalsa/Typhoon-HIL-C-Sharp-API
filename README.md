# 3. Schematic Editor API

Module: `TyphoonHil.Communication.APIs.SchematicAPI`

The Schematic API provides a set of functions/methods to manipulate existing schematic models (tse files) and create new ones programmatically. It is commonly used for creating scripts to automate repetitive tasks and for testing purposes.

## 3.1 Examples

### 3.1.1 Example 1

This example illustrates creating a model from scratch, saving it, and compiling it as the final step.

```csharp
using TyphoonHil.Communication.APIs;

namespace ConsoleApp1;

internal class Program
{
    private static void Main()
    {
        const string path = @"abs_path_to_folder + \\";

        // Create SchematicAPI object
        SchematicAPI model = new();

        // Create new model
        model.CreateNewModel("Scratch");

        // Starting coordinates
        const int x0 = 8192;
        const int y0 = 8192;

        // Component values
        const double rInValue = 100.0;
        const double lValue = 1e-5;
        const double rValue = 0.1;
        const double cValue = 5e-4;

        Console.WriteLine("Creating scheme items...");

        // Create Voltage Source component
        var vIn = model.CreateComponent("core/Voltage Source", name: "Vin", position: new Position(x0 - 300, y0),
            rotation: "right");

        // Create Resistor component
        var rIn = model.CreateComponent("core/Resistor", name: "Rin", position: new Position(x0 - 200, y0 - 100));

        // Create Current Measurement component
        var iMeas = model.CreateComponent("core/Current Measurement", name: "I",
            position: new Position(x0 - 100, y0 - 100));

        // Create Ground component
        var gnd = model.CreateComponent("core/Ground", name: "gnd", position: new Position(x0 - 300, y0 + 200));

        // Create Inductor component
        var ind = model.CreateComponent("core/Inductor", name: "L", position: new Position(x0, y0),
            rotation: Rotation.Right);

        // Create Voltage Measurement component
        var vMeas = model.CreateComponent("core/Voltage Measurement", name: "V", position: new Position(x0 + 200, y0),
            rotation: Rotation.Right);

        // Create RC Load Subsystem component
        var rcLoad =
            model.CreateComponent("core/Empty Subsystem", name: "RC Load", position: new Position(x0 + 100, y0));

        // Create port in Subsystem
        var p1 = model.CreatePort("P1", rcLoad,
            terminalPosition: new TerminalPosition(TerminalPosition.Top, TerminalPosition.Auto),
            rotation: Rotation.Right, position: new Position(x0, y0 - 200));

        // Create port in Subsystem
        var p2 = model.CreatePort("P2", rcLoad,
            terminalPosition: new TerminalPosition(TerminalPosition.Bottom, TerminalPosition.Auto),
            rotation: Rotation.Left, position: new Position(x0, y0 + 200));

        // Create Resistor component
        var r = model.CreateComponent("core/Resistor", rcLoad, "R", position: new Position(x0, y0 - 50),
            rotation: Rotation.Right);

        // Create Capacitor component
        var c = model.CreateComponent("core/Capacitor", rcLoad, "C", position: new Position(x0, y0 + 50),
            rotation: Rotation.Right);

        // Create necessary junctions
        var junction1 = model.CreateJunction("J1", position: new Position(x0 - 300, y0 + 100));
        var junction2 = model.CreateJunction("J2", position: new Position(x0, y0 - 100));
        var junction3 = model.CreateJunction("J3", position: new Position(x0, y0 + 100));
        var junction4 = model.CreateJunction("J4", position: new Position(x0 + 100, y0 - 100));
        var junction5 = model.CreateJunction("J5", position: new Position(x0 + 100, y0 + 100));

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
        const string fileName = path + "RLC_example.tse";
        Console.WriteLine($"Saving model to '{fileName}'...");
        model.SaveAs(fileName);

        // Compile model
        Console.WriteLine(model.Compile() ? "Model successfully compiled." : "Model failed to compile");

        // Close the model
        model.CloseModel();
    }
}
```
Script output:
```csharp
Creating scheme items...
Connecting components...
Setting component properties...
Saving model to 'Given-Path\RLC_example.tse'...
Model successfully compiled.
```

After executing this script, the model is saved in a file named `RLC_example.tse` in directory given in the code. Following image shows how model looks like if opened in Typhoon Schematic Editor.

![Scratch Model](Resources/scratch_model.png)
