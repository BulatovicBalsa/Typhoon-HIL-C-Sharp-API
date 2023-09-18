using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using TyphoonHil.API;

namespace TyphoonHilTests.API
{
    [TestClass()]
    public class HilAPITests
    {
        public required HilAPI Model { get; set; }
        public required string StartupPath { get; set; }
        public required string TestDataPath { get; set; }
        public required string ProtectedDataPath { get; set; }

        [TestInitialize]
        public void Init()
        {
            Model = new HilAPI();
            StartupPath = Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName;
            TestDataPath = Path.Combine(StartupPath, "TestData");
            ProtectedDataPath = Path.Combine(StartupPath, "ProtectedData");

            SchematicAPITests.ClearDirectory(TestDataPath);
        }

        [TestMethod()]
        public void SetScadaInputValueTest()
        {
            var p = new JObject() { {"result", null }};
            double? p2 = (double?)p["result"]!;
        }

        [TestMethod()]
        public void GeneralTest()
        {
            Model.LoadModel(file:Path.Combine(ProtectedDataPath, "3ph rectifier", "3ph rectifier Target files", "3ph rectifier.cpd"),
            vhilDevice: true);

            Model.LoadSettingsFile(
                Path.Combine(ProtectedDataPath, "3ph rectifier", "settings.runx"));

            Model.SetAnalogOutput(5, "V( Va )", 150.00, 5.00);

            Model.SetDigitalOutput(1, "digital input 1", true, false, 0);

            Model.SetMachineConstantTorque("machine 1", 2.5);
            Model.SetMachineLinearTorque("machine 1", 5.0);
            Model.SetMachineSquareTorque("machine 1", 6.0);
            Model.SetMachineConstantTorqueType("machine 1");
            Model.SetMachineInitialAngle("machine 1", 3.14);
            Model.SetMachineInitialSpeed("machine 1", 100.0);
            Model.SetMachineIncEncoderOffset("machine 1", 3.14);
            Model.SetMachineSinEncoderOffset("machine 1", 1.57);

            var harmonics = new List<Harmonic>() { new Harmonic(2, 23, 2) };
            Model.PrepareSourceSineWaveform(new List<string> { "Vb" }, rms: new() { 220 }, frequency: new() { 50 },
                phase: new() { 120 }, harmonics: harmonics);

            Model.PrepareSourceConstantValue("Vdc", 200);

            Model.StartSimulation();
            Assert.IsTrue(Model.IsSimulationRunning());
            
            Model.StopSimulation();
            Assert.IsFalse(Model.IsSimulationRunning());

            //Model.EnableAoLimiting(1, -1.0, 1.0, 0);
            //Model.DisableAoLimiting(1, 1);
            //Model.EndScriptByUser();
        }
    }
}