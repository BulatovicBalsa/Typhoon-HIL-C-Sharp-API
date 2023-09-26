using Microsoft.VisualStudio.TestTools.UnitTesting;
using TyphoonHil.API;

namespace TyphoonHilTests.Communication;

[TestClass]
public class NetMQTests
{
    [TestMethod]
    public void GeneralTest()
    {
        var mdl = new SchematicAPI();
        mdl.CreateNewModel("test");
    }
}