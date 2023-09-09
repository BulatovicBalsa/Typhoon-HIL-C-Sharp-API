using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using TyphoonHil.API;

namespace TyphoonHilTests.API
{
    [TestClass()]
    public class PvGeneratorAPITests
    {
        public PvGeneratorAPI Model { get; set; } = new();
        public string StartupPath { get; set; } = "";

        public string TestDataPath { get; set; } = "";
        public string ProtectedDataPath { get; set; } = "";

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
            Model = new PvGeneratorAPI();
            StartupPath = Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName;
            TestDataPath = Path.Combine(StartupPath, "TestData");
            ProtectedDataPath = Path.Combine(StartupPath, "ProtectedData");

            ClearDirectory(TestDataPath);
        }

        [TestMethod()]
        public void GeneratePvSettingsFileTest()
        {
            var PvGenerator = new PvGeneratorAPI();
            var PvParamsDetailed = new JObject
            {
                { "Voc_ref", 45.60 },
                { "Isc_ref", 5.8 },
                { "dIsc_dT", 0.0004 },
                { "Nc", 72 },
                { "dV_dI_ref", -1.1 },
                { "Vg", "cSi" },
                { "n", 1.3 },
                { "neg_current", false }
            };

            var res = PvGenerator.GeneratePvSettingsFile(PvModelType.Detailed, Path.Combine(TestDataPath, "setDet.ipvx"), PvParamsDetailed);

            Assert.IsTrue(res.Status);

            var PvParamsEN50530 = new JObject
            {
                { "Voc_ref", 45.60 },
                { "Isc_ref", 5.8 },
                { "Pv_type", "Thin film" },
                { "neg_current", false }
            };

            res = PvGenerator.GeneratePvSettingsFile(PvModelType.En50530, Path.Combine(TestDataPath, "setEN.ipvx"), PvParamsEN50530);
            Assert.IsFalse(res.Status);

            var PvParamsUserDefined = new JObject
            {
                { "Voc_ref", 45.60 },
                { "Isc_ref", 5.8 },
                { "Pv_type", "User defined" },
                { "neg_current", false },
                {
                    "user_defined_params", new JObject
                    {
                        { "ff_u", 0.72 },
                        { "ff_i", 0.8 },
                        { "c_g", 1.252e-3 },
                        { "c_v", 8.419e-2 },
                        { "c_r", 1.476e-4 },
                        { "v_l2h", 0.98 },
                        { "alpha", 0.0002 },
                        { "beta", -0.002 }
                    }
                }
            };

            res = PvGenerator.GeneratePvSettingsFile(PvModelType.NormalizedIv, Path.Combine(TestDataPath, "setIV.ipvx"), PvParamsUserDefined);
            Assert.IsFalse(res.Status);

            var PvParamsCSV = new JObject
            {
                { "csv_TestDataPath", "csv_file.csv" }
            };

            res = PvGenerator.GeneratePvSettingsFile(PvModelType.En50530, Path.Combine(TestDataPath, "setEN.csv.ipvx"), PvParamsCSV);
            Assert.IsFalse(res.Status);
        }
    }
}