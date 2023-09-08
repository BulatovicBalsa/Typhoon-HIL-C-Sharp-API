using Microsoft.VisualStudio.TestTools.UnitTesting;
using TyphoonHilApi.API;

namespace TyphoonHilApiTests.API;

[TestClass]
public class SchematicAPITests
{
    public required SchematicAPI Model { get; set; }
    public required string StartupPath { get; set; }

    public required string TestDataPath { get; set; }
    public required string ProtectedDataPath { get; set; }

    private static bool CompareFiles(string filePath1, string filePath2)
    {
        using StreamReader reader1 = new(filePath1);
        using StreamReader reader2 = new(filePath2);

        while (reader1.ReadLine() is { } line1
               && reader2.ReadLine() is { } line2)
            if (line1 != line2)
                return false;

        return reader1.ReadLine() == null && reader2.ReadLine() == null;
    }

    internal static void ClearDirectory(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            foreach (var file in Directory.GetFiles(directoryPath)) File.Delete(file);

            foreach (var subDirectory in Directory.GetDirectories(directoryPath)) ClearDirectory(subDirectory);
        }
        else
        {
            throw new DirectoryNotFoundException($"Directory '{directoryPath}' not found.");
        }
    }

    [TestInitialize]
    public void Init()
    {
        Model = new SchematicAPI();
        StartupPath = Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName;
        TestDataPath = Path.Combine(StartupPath, "TestData");
        ProtectedDataPath = Path.Combine(StartupPath, "ProtectedData");

        ClearDirectory(TestDataPath);
    }

    [TestMethod]
    public void ScratchModelTest()
    {
        var modelPath = Path.Combine(ProtectedDataPath, "RLC_example.tse");

        // Create new model
        Model.CreateNewModel("Scratch");

        // Starting coordinates
        const int x0 = 8192;
        const int y0 = 8192;

        // Component values
        const double rInValue = 100.0;
        const double lValue = 1e-5;
        const double rValue = 0.1;
        const double cValue = 5e-4;

        var vIn = Model.CreateComponent("core/Voltage Source", name: "Vin", position: new Position(x0 - 300, y0),
            rotation: "right");
        var rIn = Model.CreateComponent("core/Resistor", name: "Rin", position: new Position(x0 - 200, y0 - 100));
        var iMeas = Model.CreateComponent("core/Current Measurement", name: "I",
            position: new Position(x0 - 100, y0 - 100));
        var gnd = Model.CreateComponent("core/Ground", name: "gnd", position: new Position(x0 - 300, y0 + 200));
        var ind = Model.CreateComponent("core/Inductor", name: "L", position: new Position(x0, y0),
            rotation: Rotation.Right);
        var vMeas = Model.CreateComponent("core/Voltage Measurement", name: "V", position: new Position(x0 + 200, y0),
            rotation: Rotation.Right);
        var rcLoad =
            Model.CreateComponent("core/Empty Subsystem", name: "RC Load", position: new Position(x0 + 100, y0));
        var p1 = Model.CreatePort("P1", rcLoad,
            terminalPosition: new TerminalPosition(TerminalPosition.Top, TerminalPosition.Auto),
            rotation: Rotation.Right, position: new Position(x0, y0 - 200));
        var p2 = Model.CreatePort("P2", rcLoad,
            terminalPosition: new TerminalPosition(TerminalPosition.Bottom, TerminalPosition.Auto),
            rotation: Rotation.Left, position: new Position(x0, y0 + 200));
        var r = Model.CreateComponent("core/Resistor", rcLoad, "R", position: new Position(x0, y0 - 50),
            rotation: Rotation.Right);
        var c = Model.CreateComponent("core/Capacitor", rcLoad, "C", position: new Position(x0, y0 + 50),
            rotation: Rotation.Right);
        var junction1 = Model.CreateJunction("J1", position: new Position(x0 - 300, y0 + 100));
        var junction2 = Model.CreateJunction("J2", position: new Position(x0, y0 - 100));
        var junction3 = Model.CreateJunction("J3", position: new Position(x0, y0 + 100));
        var junction4 = Model.CreateJunction("J4", position: new Position(x0 + 100, y0 - 100));
        var junction5 = Model.CreateJunction("J5", position: new Position(x0 + 100, y0 + 100));

        Model.CreateConnection(Model.Term(vIn, "p_node"), Model.Term(rIn, "p_node"));
        Model.CreateConnection(Model.Term(vIn, "n_node"), junction1);
        Model.CreateConnection(Model.Term(gnd, "node"), junction1);
        Model.CreateConnection(Model.Term(rIn, "n_node"), Model.Term(iMeas, "p_node"));
        Model.CreateConnection(Model.Term(iMeas, "n_node"), junction2);
        Model.CreateConnection(junction2, Model.Term(ind, "p_node"));
        Model.CreateConnection(Model.Term(ind, "n_node"), junction3);
        Model.CreateConnection(junction1, junction3);
        Model.CreateConnection(junction2, junction4);
        Model.CreateConnection(junction3, junction5);
        Model.CreateConnection(Model.Term(rcLoad, "P1"), junction4);
        Model.CreateConnection(junction5, Model.Term(rcLoad, "P2"));
        Model.CreateConnection(junction4, Model.Term(vMeas, "p_node"));
        Model.CreateConnection(Model.Term(vMeas, "n_node"), junction5);
        Model.CreateConnection(p1, Model.Term(r, "p_node"));
        Model.CreateConnection(Model.Term(r, "n_node"), Model.Term(c, "p_node"));
        Model.CreateConnection(Model.Term(c, "n_node"), p2);

        Model.SetPropertyValue(Model.Prop(rIn, "resistance"), rInValue);
        Model.SetPropertyValue(Model.Prop(ind, "inductance"), lValue);
        Model.SetPropertyValue(Model.Prop(r, "resistance"), rValue);
        Model.SetPropertyValue(Model.Prop(c, "capacitance"), cValue);

        var filePath = Path.Combine(TestDataPath, "RLC_example.tse");
        Model.SaveAs(filePath);

        Assert.IsTrue(Model.Compile());

        Model.CloseModel();

        Assert.IsTrue(CompareFiles(filePath, modelPath));
    }

    [TestMethod]
    public void CreateCommentTest()
    {
        Model.CreateNewModel();

        var comment2 = Model.CreateComment("This is a comment 2", name: "Comment 2", position: new Position(100, 200));

        Model.CloseModel();

        Assert.IsNotNull(comment2);
        Assert.AreEqual(comment2["fqn"], "Comment 2");
    }

    [TestMethod]
    public void CreateLibraryTest()
    {
        var libraryPath = Path.Combine(ProtectedDataPath, "example_library.tlib");

        var filePath = Path.Combine(
            TestDataPath,
            "create_library_model_lib",
            "example_library.tlib"
        );

        const string libName = "Example Library";

        Model.CreateLibraryModel(libName, filePath);

        var r = Model.CreateComponent("core/Resistor", name: "R1");
        var c = Model.CreateComponent("core/Capacitor", name: "C1");
        Model.CreateConnection(Model.Term(c, "n_node"), Model.Term(r, "p_node"));
        Model.CreateConnection(Model.Term(c, "p_node"), Model.Term(r, "n_node"));

        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        Model.SaveAs(filePath);
        Model.Load(filePath);
        Model.Save();

        Assert.IsTrue(CompareFiles(filePath, libraryPath));
    }
}