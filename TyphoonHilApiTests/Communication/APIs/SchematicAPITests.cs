using Microsoft.VisualStudio.TestTools.UnitTesting;
using TyphoonHilApi.Communication.APIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;

namespace TyphoonHilApi.Communication.APIs.Tests
{
    [TestClass]
    public class SchematicAPITests
    {
        public SchematicAPI Model { get; set; }
        public string StartupPath { get; set; }

        public string TestDataPath { get; set; }
        public string ProtectedDataPath { get; set; }

        bool CompareFiles(string filePath1, string filePath2)
        {
            using (StreamReader reader1 = new(filePath1))
            using (StreamReader reader2 = new(filePath2))
            {
                string? line1, line2;

                while ((line1 = reader1.ReadLine()) != null
                    && (line2 = reader2.ReadLine()) != null)
                {
                    if (line1 != line2)
                    {
                        return false;
                    }
                }

                if (reader1.ReadLine() != null || reader2.ReadLine() != null)
                {
                    return false;
                }

                return true;
            }
        }

        void ClearDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                foreach (var file in Directory.GetFiles(directoryPath))
                {
                    File.Delete(file);
                }

                foreach (var subdirectory in Directory.GetDirectories(directoryPath))
                {
                    ClearDirectory(subdirectory);
                }
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
            string modelPath = Path.Combine(ProtectedDataPath, "RLC_example.tse");

            // Create new model
            Model.CreateNewModel("Scratch");

            // Starting coordinates
            int x0 = 8192;
            int y0 = 8192;

            // Component values
            double rInValue = 100.0;
            double lValue = 1e-5;
            double rValue = 0.1;
            double cValue = 5e-4;

            JObject vIn = Model.CreateComponent("core/Voltage Source", name: "Vin", position: new(x0 - 300, y0), rotation: "right");
            JObject rIn = Model.CreateComponent("core/Resistor", name: "Rin", position: new(x0 - 200, y0 - 100));
            JObject iMeas = Model.CreateComponent("core/Current Measurement", name: "I", position: new(x0 - 100, y0 - 100));
            JObject gnd = Model.CreateComponent("core/Ground", name: "gnd", position: new(x0 - 300, y0 + 200));
            JObject ind = Model.CreateComponent("core/Inductor", name: "L", position: new(x0, y0), rotation: Rotation.Right);
            JObject vMeas = Model.CreateComponent("core/Voltage Measurement", name: "V", position: new(x0 + 200, y0), rotation: Rotation.Right);
            JObject rcLoad = Model.CreateComponent("core/Empty Subsystem", name: "RC Load", position: new(x0 + 100, y0));
            JObject p1 = Model.CreatePort(name: "P1", parent: rcLoad, terminalPosition: new TerminalPosition(TerminalPosition.Top, TerminalPosition.Auto), rotation: Rotation.Right, position: new(x0, y0 - 200));
            JObject p2 = Model.CreatePort(name: "P2", parent: rcLoad, terminalPosition: new TerminalPosition(TerminalPosition.Bottom, TerminalPosition.Auto), rotation: Rotation.Left, position: new(x0, y0 + 200));
            JObject r = Model.CreateComponent("core/Resistor", parent: rcLoad, name: "R", position: new(x0, y0 - 50), rotation: Rotation.Right);
            JObject c = Model.CreateComponent("core/Capacitor", parent: rcLoad, name: "C", position: new(x0, y0 + 50), rotation: Rotation.Right);
            JObject junction1 = Model.CreateJunction(name: "J1", position: new(x0 - 300, y0 + 100));
            JObject junction2 = Model.CreateJunction(name: "J2", position: new(x0, y0 - 100));
            JObject junction3 = Model.CreateJunction(name: "J3", position: new(x0, y0 + 100));
            JObject junction4 = Model.CreateJunction(name: "J4", position: new(x0 + 100, y0 - 100));
            JObject junction5 = Model.CreateJunction(name: "J5", position: new(x0 + 100, y0 + 100));

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

            string filePath = Path.Combine(TestDataPath, "RLC_example.tse");
            Model.SaveAs(filePath);

            Assert.IsTrue(Model.Compile());
            
            Model.CloseModel();

            Assert.IsTrue(CompareFiles(filePath, modelPath));
        }

        [TestMethod]
        public void CreateCommentTest()
        {
            Model.CreateNewModel();

            JObject comment2 = Model.CreateComment("This is a comment 2", name: "Comment 2", position: new(100, 200));

            Model.CloseModel();

            Assert.IsNotNull(comment2);
            Assert.AreEqual(comment2["fqn"], "Comment 2");
        }

        [TestMethod]
        public void CreateLibraryTest()
        {
            string libraryPath = Path.Combine(ProtectedDataPath, "example_library.tlib");

            string filePath = Path.Combine(
                TestDataPath,
                "create_library_model_lib",
                "example_library.tlib"
            );

            string libName = "Example Library";

            Model.CreateLibraryModel(libName, filePath);

            JObject r = Model.CreateComponent("core/Resistor", name: "R1");
            JObject c = Model.CreateComponent("core/Capacitor", name: "C1");
            JObject con = Model.CreateConnection(Model.Term(c, "n_node"), Model.Term(r, "p_node"));
            JObject con1 = Model.CreateConnection(Model.Term(c, "p_node"), Model.Term(r, "n_node"));

            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            }
            Model.SaveAs(filePath);
            Model.Load(filePath);
            Model.Save();

            Assert.IsTrue(CompareFiles(filePath, libraryPath));
        }
    }
}