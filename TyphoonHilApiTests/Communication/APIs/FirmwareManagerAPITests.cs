using Microsoft.VisualStudio.TestTools.UnitTesting;
using TyphoonHilApi.Communication.APIs;

namespace TyphoonHilApiTests.Communication.APIs
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