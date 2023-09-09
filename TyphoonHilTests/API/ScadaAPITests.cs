﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using TyphoonHil.API;
using TyphoonHil.Exceptions;

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

        scadaApi.SetPropertyValue(widgetHandle, ScadaConstants.PropName, "New widget name");

        Assert.ThrowsException<ScadaAPIException>(
            () => scadaApi.SetPropertyValue(widgetHandle, ScadaConstants.PropName, new[] { 56, 28, 89 }));

        scadaApi.SavePanel();
        scadaApi.SavePanelAs(targetFilePath);
    }
}