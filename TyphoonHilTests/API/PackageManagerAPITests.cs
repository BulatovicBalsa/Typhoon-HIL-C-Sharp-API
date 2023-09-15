using Microsoft.VisualStudio.TestTools.UnitTesting;
using TyphoonHil.API;

namespace TyphoonHilTests.API;

[TestClass]
public class PackageManagerAPITests
{
    public required PackageManagerAPI Model { get; set; }
    public required string StartupPath { get; set; }

    public required string TestDataPath { get; set; }
    public required string ProtectedDataPath { get; set; }

    [TestInitialize]
    public void Init()
    {
        Model = new();
        StartupPath = Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName;
        TestDataPath = Path.Combine(StartupPath, "TestData");
        ProtectedDataPath = Path.Combine(StartupPath, "ProtectedData");

        SchematicAPITests.ClearDirectory(TestDataPath);
    }

    [TestMethod]
    public void CreateExampleTest()
    {
        var title = "Example 3ph rectifier";
        var modelFile = Path.Combine(ProtectedDataPath, "3ph rectifier pkm", "rectifier.tse");
        var panelFile = Path.Combine(ProtectedDataPath, "3ph rectifier pkm", "rectifier.cus");
        var outputPath = Path.Combine(ProtectedDataPath, "3ph rectifier pkm", "output");
        var tags = new List<string> {"rectifier", "example"};
        var description = "This example demonstrates a three-phase diode rectifier connected to the grid.";
        var imageFile = Path.Combine(ProtectedDataPath, "3ph rectifier pkm", "rectifier.svg");
        //var appNoteFile = Path.Combine("3ph rectifier pkm", "rectifier.html");
        //var tests = [Path.Combine(ProtectedDataPath, "package data", "example_tests")];
        //var testResources = [];
        var resources = new List<string> {Path.Combine(ProtectedDataPath, "3ph rectifier pkm", "data.json")};

        var example = Model.CreateExample(title, modelFile, panelFile, outputPath, tags, description, imageFile, resources:resources);
        Directory.Delete(example, true);
    }

    [TestMethod]
    public void GetInstalledPackagesTest()
    {
        Console.WriteLine(Model.GetInstalledPackages());
        Console.WriteLine(Model.GetModifiedPackages());
    }


    [TestMethod]
    public void CreatePackageTest()
    {
        var packageName = "An example package";
        var version = "1.0.5";
        var author = "Typhoon HIL";
        var description = "An example package demonstrating API functionality.";
        var libraryPaths = Path.Combine(ProtectedDataPath, "package data", "libs");
        var resourcePaths = new List<string>();
        var examplePaths = Path.Combine(ProtectedDataPath, "package data", "examples");
        var additionalFilesPaths = Path.Combine(ProtectedDataPath, "package data", "additional.txt");
        var documentationPaths = Path.Combine(ProtectedDataPath, "package data", "documentation");
        var documentationLandingPage = Path.Combine(ProtectedDataPath, "package data", "documentation", "index.html");
        var releaseNotesPath = Path.Combine(ProtectedDataPath, "package data", "release_notes.html");

        var pythonPackagesPaths = new List<string>();

        var outputPath = Path.Combine(ProtectedDataPath, "package data", "output");

        var packagePath = Model.CreatePackage(packageName, version, outputPath, author, description);

        Model.ValidatePackage(packagePath);
        Console.WriteLine("Package is valid");

        Model.InstallPackage(packagePath);
        Model.GetInstalledPackages().ForEach(Console.WriteLine);

        Model.ReinstallPackage(packageName);

        foreach (var package in Model.GetInstalledPackages())
        {
            Model.UninstallPackage(package.Name);
        }
        Model.GetInstalledPackages().ForEach(Console.WriteLine);
    }
}