using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using TyphoonHil.API;
using TyphoonHil.Exceptions;

namespace TyphoonHilTests.API;

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

    [TestMethod]
    public void AddLibraryPathTest()
    {
        Model.CreateNewModel();

        var libPath = Path.Combine(ProtectedDataPath, "user_lib");

        // Get all current library paths and remove them
        var oldPaths = Model.GetLibraryPaths();
        oldPaths.ForEach(x => Model.RemoveLibraryPath(x));

        Assert.IsTrue(Model.GetLibraryPaths().Count == 0);

        // Add library path and reload library to be able to use the added library.
        Model.AddLibraryPath(libPath);
        Model.ReloadLibraries();

        // Create components from loaded libraries.
        var comp = Model.CreateComponent("User Component Library/NPC PV Inverter");
        Assert.ThrowsException<SchematicAPIException>(() => Model.CreateComponent("Non existing comp"));

        // Remove library from the path.
        Model.RemoveLibraryPath(libPath);

        // Add again the previous library paths
        foreach (var path in oldPaths) Model.AddLibraryPath(path);

        Model.CloseModel();
    }

    [TestMethod]
    public void CreateMaskTest()
    {
        Model.CreateNewModel();
        var sub = Model.CreateComponent("core/Subsystem", name: "Sb1");
        var mask = Model.CreateMask(sub);

        Assert.AreEqual(mask["fqn"]!, "Sb1.Mask@top");

        Model.CloseModel();
    }

    [TestMethod]
    public void CreatePropertyTest()
    {
        Model.CreateNewModel();

        var sub = Model.CreateComponent("core/Subsystem", name: "Sub1");

        var mask = Model.CreateMask(sub);

        var prop1 = Model.CreateProperty(
            mask, "prop_1", "Property 1",
            Widget.Combo,
            new JArray { "Choice 1", "Choice 2", "Choice 3" },
            tabName: "First tab"
        );

        var prop2 = Model.CreateProperty(
            mask, "prop_2", "Property 2",
            Widget.Button, tabName: "Second tab"
        );

        Model.RemoveProperty(mask, "prop_2");

        Model.CloseModel();

        Assert.IsTrue((bool)prop1["item_handle"]!);
        Assert.IsTrue((bool)prop2["item_handle"]!);
    }

    [TestMethod]
    public void CreateTagTest()
    {
        Model.CreateNewModel();

        var tag = Model.CreateTag(name: "Tag 1", value: "Tag value", position: new Position(160, 240));
        Assert.AreEqual(Model.GetPosition(tag), new Position(160, 240));

        Model.SetPosition(tag, new Position(800, 1600));
        Assert.AreEqual(Model.GetPosition(tag), new Position(800, 1600));

        Model.CloseModel();
    }

    [TestMethod]
    public void DeleteItemTest()
    {
        Model.CreateNewModel();

        // Create some items and then delete them.
        var r = Model.CreateComponent("core/Resistor");
        var j = Model.CreateJunction();
        // var tag = mdl.CreateTag(value: "Val 1");
        var sub1 = Model.CreateComponent("core/Subsystem");
        var innerPort = Model.CreatePort(parent: sub1, name: "Inner port1");

        //
        // Delete items
        //
        Model.DeleteItem(r);
        Model.DeleteItem(j);
        //mdl.DeleteItem(tag);

        // Delete subsystem
        Model.DeleteItem(sub1);

        Model.CloseModel();
    }

    [TestMethod]
    public void DetectHwTest()
    {
        Model.CreateNewModel();

        var hwSett = Model.DetectHwSettings();

        if (hwSett != null)
            Console.WriteLine("HIL device was detected and model configuration was changed to {0}.", hwSett);
        else
            Console.WriteLine("HIL device autodetection failed, maybe HIL device is not connected.");

        Model.CloseModel();
    }

    [TestMethod]
    public void DisableItemsTest()
    {
        var modelPath = Path.Combine(ProtectedDataPath, "RLC_example.tse");
        File.Copy(modelPath, modelPath.Replace("ProtectedData", "TestData"));

        Model.Load(Path.Combine(TestDataPath, "RLC_example.tse"));

        //
        // Names of items that can be disabled
        //
        List<string> itemDisableNames = new()
        {
            "L",
            "AI_1",
            "AI_2",
            "SM_1",
            "Probe1"
        };

        //
        // Names of subsystem [0] and item [1] inside subsystem
        //
        var subsystemName = "SS_1";
        var itemNameInsideSubsystem = "SM_6";

        //
        // Names of items that cannot be disabled
        //
        List<string> itemDontDisableNames = new()
        {
            "Subsystem1",
            "SM_5",
            "Min Max 1",
            "GA_2"
        };

        //
        // Fetch all items that can be disabled and that cannot be disabled
        //
        var itemsDisable = itemDisableNames.Select(itemName => Model.GetItem(itemName)).ToList();
        var itemsDontDisable = itemDontDisableNames.Select(itemName => Model.GetItem(itemName)).ToList();

        //
        // Disable, compile, enable - items that can be disabled
        //
        var disabledItems = Model.DisableItems(itemsDisable);
        Model.Compile();
        var affectedItems = Model.EnableItems(disabledItems);
        Model.Compile();

        //
        // Disable, compile, enable - items that cannot be disabled
        //
        disabledItems = Model.DisableItems(itemsDontDisable);
        try
        {
            Model.Compile();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        affectedItems = Model.EnableItems(disabledItems!);
        Model.Compile();

        //
        // Disable, compile, enable - items inside subsystem
        //
        var parentItem = Model.GetItem(subsystemName);
        var concreteItem = Model.GetItem(itemNameInsideSubsystem, parentItem);
        disabledItems = Model.DisableItems(new List<JObject?> { concreteItem });
        try
        {
            Model.Compile();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        affectedItems = Model.EnableItems(new List<JObject?> { concreteItem });
        Model.Compile();

        Model.CloseModel();
    }

    [TestMethod]
    public void DisablePropertyTest()
    {
        Model.CreateNewModel();

        // Create component
        var r = Model.CreateComponent("core/Resistor");

        // Disable property
        Model.DisableProperty(Model.Prop(r, "resistance"));

        // Check to see if property is enabled.
        Console.WriteLine(Model.IsPropertyEnabled(Model.Prop(r, "resistance")));

        // Enable property
        Model.EnableProperty(Model.Prop(r, "resistance"));

        Console.WriteLine(Model.IsPropertyEnabled(Model.Prop(r, "resistance")));

        Model.CloseModel();
    }

    [TestMethod]
    public void DisablePropertySerializationTest()
    {
        Model.CreateNewModel();

        var constComponent = Model.CreateComponent("core/Constant");

        Assert.IsTrue(Model.IsPropertySerializable(Model.Prop(constComponent, "value")));

        Model.DisablePropertySerialization(Model.Prop(constComponent, "value"));
        Assert.IsFalse(Model.IsPropertySerializable(Model.Prop(constComponent, "value")));

        Model.EnablePropertySerialization(Model.Prop(constComponent, "value"));
        Assert.IsTrue(Model.IsPropertySerializable(Model.Prop(constComponent, "value")));

        Model.CloseModel();
    }

    [TestMethod]
    public void DisplayComponentIconTextTest()
    {
        Model.CreateNewModel();
        var tr1 = Model.CreateComponent("core/Three Phase Two Winding Transformer");
        Model.SetComponentIconImage(tr1, Path.Combine(ProtectedDataPath, "transformer.png"));

        Model.SetColor(tr1, "red");
        Model.DisplayComponentIconText(tr1, "Sample text");

        Model.RefreshIcon(tr1);

        Model.CloseModel();
    }

    [TestMethod]
    public void ErrorTest()
    {
        Model.CreateNewModel();

        var ct = Model.CreateComponent("core/Constant", name: "Constant 1");
        Model.Error("Some error", context: ct);

        Model.CloseModel();
    }

    [TestMethod]
    public void ExistsTest()
    {
        Model.CreateNewModel();

        var r = Model.CreateComponent("core/Resistor", name: "R 1");
        Assert.IsTrue(Model.Exists("R 1"));

        var sub1 = Model.CreateComponent("core/Subsystem");
        var innerC = Model.CreateComponent("core/Capacitor", sub1, "Capacitor 1");

        Assert.IsFalse(Model.Exists("Capacitor  1", sub1));

        var sub2 = Model.CreateComponent("core/Subsystem", name: "Sub 2");
        Model.CreatePort("Port 2", sub2);

        Model.SaveAs(Path.Combine(TestDataPath, "tmp.tse"));
        Assert.IsTrue(Model.Exists("Port 2", sub2, ItemType.Terminal));

        Model.CloseModel();
    }

    // Takes too much time for run
    public void ExportCTest()
    {
        Model.CreateNewModel();
        var constant = Model.CreateComponent("core/Constant");
        var probe1 = Model.CreateComponent("core/Probe");
        var probe2 = Model.CreateComponent("core/Probe");
        var subsystem = Model.CreateComponent("core/Empty Subsystem");

        var subIn = Model.CreatePort(parent: subsystem,
            direction: Direction.In,
        kind: Kind.Sp,
        name: "in");

        var subOut = Model.CreatePort(parent: subsystem,
            kind: Kind.Sp,
        direction: Direction.Out,
        name: "out");

        var subOut1 = Model.CreatePort(parent: subsystem,
            kind: Kind.Sp,
            direction: Direction.Out,
            name: "out1");
        var subSum = Model.CreateComponent("core/Sum", parent:  subsystem);
        var subConst = Model.CreateComponent("core/Constant", parent:  subsystem);
        var subGain = Model.CreateComponent("core/Gain", parent:  subsystem);
        var subJ = Model.CreateJunction(kind: Kind.Sp, parent:  subsystem);

        Model.CreateConnection(Model.Term(constant, "out"),
            Model.Term(subsystem, "in"));

        Model.CreateConnection(subIn,
            Model.Term(subSum, "in"));

        Model.CreateConnection(Model.Term(subConst, "out"),
            Model.Term(subSum, "in1"));

        Model.CreateConnection(Model.Term(subSum, "out"),
            subJ);

        Model.CreateConnection(subJ,
            Model.Term(subGain, "in"));

        Model.CreateConnection(Model.Term(subGain, "out"),
            subOut);

        Model.CreateConnection(subJ, subOut1);

        Model.CreateConnection(Model.Term(subsystem, "out"), 
            Model.Term(probe1, "in"));

        Model.CreateConnection(Model.Term(subsystem, "out1"),
            Model.Term(probe2, "in"));

        var outputDir = Path.Combine(TestDataPath, "exportC");

        Model.ExportCFromSubsystem(subsystem, outputDir);

        Model.CloseModel();
    }

    [TestMethod]
    public void ExportToJsonTest()
    {
        if (File.Exists(Path.Combine(TestDataPath, "RLC_example.json"))) File.Delete(Path.Combine(TestDataPath, "RLC_example.json"));
        var path = Path.Combine(ProtectedDataPath, "RLC_example.tse");
        Model.Load(path);
        Model.ExportModelToJson(TestDataPath);
        Model.CloseModel();

        Assert.IsTrue(File.Exists(Path.Combine(TestDataPath, "RLC_example.json")));
    }

    [TestMethod]
    public void FindConnectionsTest()
    {
        Model.CreateNewModel();

        var const1 = Model.CreateComponent("core/Constant", name: "Constant 1");
        var junction = Model.CreateJunction(kind: Kind.Sp, name: "Junction 1");
        var probe1 = Model.CreateComponent("core/Probe", name: "Probe 1");
        var probe2 = Model.CreateComponent("core/Probe", name: "Probe 2");
        var con1 = Model.CreateConnection(Model.Term(const1, "out"), junction);
        var con2 = Model.CreateConnection(junction, Model.Term(probe1, "in"));
        var con3 = Model.CreateConnection(junction, Model.Term(probe2, "in"));

        Model.FindConnections(junction).ForEach(Console.WriteLine);
        Console.WriteLine("Another one");
        Model.FindConnections(junction, Model.Term(probe2, "in")).ForEach(Console.WriteLine);

        Model.CloseModel();
    }

    [TestMethod]
    public void FqnTest()
    {
        Model.CreateNewModel();

        const string parentName = "Subsystem 1";
        const string componentName = "Resistor 1";

        var compFqn = SchematicAPI.Fqn(parentName, componentName);
        Assert.AreEqual(compFqn, componentName);

        Model.GetAvailableLibraryComponents().ForEach(Console.WriteLine);


        Model.CloseModel();
    }

    [TestMethod]
    public void GetBreakpointsTest()
    {
        Model.CreateNewModel();

        var constComponent = Model.CreateComponent("core/Constant");
        var probeComponent = Model.CreateComponent("core/Probe");
        var connection = Model.CreateConnection(
            Model.Term(constComponent, "out"),
            Model.Term(probeComponent, "in"),
            breakpoints: new List<Position>
            {
                new(100, 200),
                new(100, 0)
            }
        );

        var breakpoints = Model.GetBreakpoints(connection);

        Model.CloseModel();

        Assert.IsTrue(breakpoints.Contains(new Position(100, 200)));
        Assert.IsTrue(breakpoints.Contains(new Position(100, 0)));
    }

    [TestMethod]
    public void CommentsTest()
    {
        Model.CreateNewModel();
        var comment = Model.CreateComment("Comm1");
        Assert.AreEqual("Comm1", Model.GetCommentText(comment));

        Model.CloseModel();
    }

    [TestMethod]
    public void GetCommentsTest()
    {
        var cpd = Model.GetCompiledModelFile(Path.Combine(ProtectedDataPath, "RLC_example.tse"));
        Console.WriteLine(cpd);
    }

    [TestMethod]
    public void ComponentTest()
    {
        Model.CreateNewModel();
        var r = Model.CreateComponent("core/Resistor");
        Assert.AreEqual((Model.GetName(r), Model.GetComponentTypeName(r)), ("R1", "pas_resistor"));
        Assert.AreEqual(Model.GetConnectableKind(Model.Term(r, "p_node")), "pe");
        Console.WriteLine(Model.GetConvProp(Model.Prop(r, "resistance"),"234.1"));
        Assert.AreEqual("R1", Model.GetFqn(r));

        var cc = Model.CreateComponent("Single Phase Core Coupling");
        Assert.AreEqual((Model.GetName(cc), Model.GetComponentTypeName(cc)), ("Core Coupling 1", "Single Phase Core Coupling"));

        var sub = Model.CreateComponent("core/Subsystem");
        var mask = Model.CreateMask(sub);
        Model.SetDescription(mask, "Mask desc");
        Assert.AreEqual((Model.GetName(sub), Model.GetComponentTypeName(sub)), ("Subsystem1", ""));
        Assert.AreEqual(Model.GetDescription(mask), "Mask desc");

        var ind = Model.CreateComponent("core/Inductor", parent: sub, name: "L1");
        Assert.AreEqual(Model.GetFqn(ind), "Subsystem1.L1");
        Model.SetName(ind, "New name");
        Assert.AreEqual(Model.GetFqn(ind), "Subsystem1.New name");

        var ct = Model.CreateComponent("core/Constant", name: "Constant 1");
        Assert.AreEqual(Model.GetConnectableDirection(Model.Term(ct, "out")), "out");
        Assert.AreEqual(Model.GetConnectableKind(Model.Term(ct, "out")), "sp");

        var probe = Model.CreateComponent("core/Probe", name: "Probe 1");
        Assert.AreEqual(Model.GetConnectableDirection(Model.Term(probe, "in")), "in");


        var const1 = Model.CreateComponent("core/Constant", name: "Constant1");
        var const2 = Model.CreateComponent("core/Constant", name: "Constant2");
        var junction = Model.CreateJunction(kind: Kind.Sp);
        var sum1 = Model.CreateComponent("core/Sum", name: "Sum1");
        var probe1 = Model.CreateComponent("core/Probe", name: "Probe1");
        var probe2 = Model.CreateComponent("core/Probe", name: "Probe2");

        var con1 = Model.CreateConnection(Model.Term(const1, "out"), junction);
        var con2 = Model.CreateConnection(junction, Model.Term(probe2, "in"));
        var con3 = Model.CreateConnection(junction, Model.Term(sum1, "in"));
        var con4 = Model.CreateConnection(Model.Term(const2, "out"), Model.Term(sum1, "in1"));
        var con5 = Model.CreateConnection(Model.Term(sum1, "out"), Model.Term(probe1, "in"));

        Assert.IsTrue(Model.GetConnectedItems(const1).Select(Model.GetName).All(new List<string>{"Probe2", "Sum1"}.Contains));
        Assert.IsTrue(Model.GetConnectedItems(junction).Select(Model.GetName).All(new List<string>{"Probe2", "Sum1", "Constant1"}.Contains));
        Assert.IsTrue(Model.GetConnectedItems(Model.Term(sum1, "out")).Select(Model.GetName).All(new List<string>{"Probe1"}.Contains));

        Model.CloseModel();
    }
}