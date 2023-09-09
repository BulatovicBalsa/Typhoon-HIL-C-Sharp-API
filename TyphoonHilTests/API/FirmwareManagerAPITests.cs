using Microsoft.VisualStudio.TestTools.UnitTesting;
using TyphoonHil.API;

namespace TyphoonHilTests.API
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