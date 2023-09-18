using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using TyphoonHil.API;

namespace TyphoonHilTests.API
{
    [TestClass]
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

        [TestMethod]
        public void GeneratePvSettingsFileTest()
        {
            var pvGenerator = new PvGeneratorAPI();
            var pvParamsDetailed = new JObject
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

            var res = pvGenerator.GeneratePvSettingsFile(PvModelType.Detailed, Path.Combine(TestDataPath, "setDet.ipvx"), pvParamsDetailed);

            Assert.IsTrue(res.Status);

            var pvParamsEn50530 = new JObject
            {
                { "Voc_ref", 45.60 },
                { "Isc_ref", 5.8 },
                { "Pv_type", "Thin film" },
                { "neg_current", false }
            };

            res = pvGenerator.GeneratePvSettingsFile(PvModelType.En50530, Path.Combine(TestDataPath, "setEN.ipvx"), pvParamsEn50530);
            Assert.IsFalse(res.Status);

            var pvParamsUserDefined = new JObject
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

            res = pvGenerator.GeneratePvSettingsFile(PvModelType.NormalizedIv, Path.Combine(TestDataPath, "setIV.ipvx"), pvParamsUserDefined);
            Assert.IsFalse(res.Status);

            var pvParamsCsv = new JObject
            {
                { "csv_TestDataPath", "csv_file.csv" }
            };

            res = pvGenerator.GeneratePvSettingsFile(PvModelType.En50530, Path.Combine(TestDataPath, "setEN.csv.ipvx"), pvParamsCsv);
            Assert.IsFalse(res.Status);
        }
    }
}