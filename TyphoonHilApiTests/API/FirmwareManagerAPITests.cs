using Microsoft.VisualStudio.TestTools.UnitTesting;
using TyphoonHilApi.API;

namespace TyphoonHilApiTests.API
{
    [TestClass()]
    public class FirmwareManagerAPITests
    {
        [TestMethod()]
        public void GetHilInfoTest()
        {
            var info = new FirmwareManagerAPI().GetHilInfo();
        }
    }
}