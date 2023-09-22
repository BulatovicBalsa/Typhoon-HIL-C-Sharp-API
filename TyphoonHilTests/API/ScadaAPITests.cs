using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TyphoonHil.API;
using TyphoonHil.Exceptions;
using Path = System.IO.Path;

namespace TyphoonHilTests.API;

[TestClass]
public class ScadaAPITests
{
    public required ScadaAPI Model { get; set; }
    public required string StartupPath { get; set; }
    public required string TestDataPath { get; set; }
    public required string ProtectedDataPath { get; set; }

    [TestInitialize]
    public void Init()
    {
        Model = new ScadaAPI();
        StartupPath = Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName;
        TestDataPath = Path.Combine(StartupPath, "TestData");
        ProtectedDataPath = Path.Combine(StartupPath, "ProtectedData");

        SchematicAPITests.ClearDirectory(TestDataPath);
    }

    [TestMethod]
    public void AddLibraryPathTest()
    {
        Assert.IsFalse(false);
    }

    [TestMethod]
    public void GeneralTest()
    {
        const string panelName = "3ph rectifier.cus";
        var targetFilePath = Path.Combine(ProtectedDataPath, "3ph rectifier", panelName);
        var scadaApi = new ScadaAPI();
        Assert.ThrowsException<ScadaAPIException>(() => scadaApi.LoadPanel("not_existing_file_path"));
        scadaApi.LoadPanel(targetFilePath);

        var widgetHandle = scadaApi.GetWidgetById("76555d9ee1ac11e7b3407085c23c3b8d");
        Assert.IsNotNull(widgetHandle);

        scadaApi.SetPropertyValue(widgetHandle, ScadaConstants.Name, "New widget name");
        Assert.AreEqual(Model.GetPropertyValue(widgetHandle, ScadaConstants.Name), "New widget name");

        Assert.ThrowsException<ScadaAPIException>(
            () => scadaApi.SetPropertyValue(widgetHandle, ScadaConstants.Name, new[] { 56, 28, 89 }));

        scadaApi.SavePanel();
        scadaApi.SavePanelAs(targetFilePath);
    }

    [TestMethod]
    public void LibraryTests()
    {
        Model.AddLibraryPath(Path.Combine(ProtectedDataPath, "user_lib"));
        Model.ReloadLibraries();
        Model.RemoveLibraryPath(Path.Combine(ProtectedDataPath, "user_lib"));
        Model.GetLibraryPaths().ForEach(Console.WriteLine);
    }

    [TestMethod]
    public void WidgetTests()
    {
        Model.CreateNewPanel();

        var groupHandle = Model.CreateWidget(widgetType: ScadaConstants.WtGroup, name: "Group for other widgets",
            position: new(0, 200));

        var digDHandle = Model.CreateWidget(widgetType: ScadaConstants.WtDigital, parent: groupHandle,
            name: "Digital Display", position: new(20, 20));

    }

    [TestMethod]
    public void PanelTests()
    {
        Model.CreateNewPanel();
        var digDHandle = Model.CreateWidget(widgetType: ScadaConstants.WtDigital,
            name: "Digital Display", position: new(20, 20));

        Model.SavePanelAs(Path.Combine(TestDataPath, "panel.cus"));
        Model.SavePanel();
    }

    [TestMethod]
    public void LibraryPanelTest()
    {
        Model.CreateNewLibraryPanel("Lib1", "My simple lib");
        
        var groupHandle = Model.CreateWidget(widgetType: ScadaConstants.WtGroup, name: "Group for other widgets",
            position: new(0, 200));

        var digDHandle = Model.CreateWidget(widgetType: ScadaConstants.WtDigital, parent: groupHandle,
            name: "Digital Display", position: new(20, 20));

        Model.SavePanelAs(Path.Combine(TestDataPath, "lib1.wlib"));
        Model.LoadLibraryPanel(Path.Combine(TestDataPath, "lib1.wlib"));
    }

    [TestMethod]
    public void CopyTest()
    {
        Model.CreateNewPanel();
        var groupHandle = Model.CreateWidget(widgetType: ScadaConstants.WtGroup, name: "Group for other widgets",
            position: new(0, 200));

        var digDHandle = Model.CreateWidget(widgetType: ScadaConstants.WtDigital,
            name: "Digital Display", position: new(20, 20));

        var copiedWidgets = Model.Copy(srcHandle: digDHandle, dstHandle:groupHandle, name: "New name", position:new(0, 304));
        Console.WriteLine(copiedWidgets);

        copiedWidgets = Model.Copy(srcHandle: digDHandle, name: "New name", position: new(0, 304));
        Console.WriteLine(copiedWidgets);

        Model.DeleteWidget(digDHandle);
    }
}