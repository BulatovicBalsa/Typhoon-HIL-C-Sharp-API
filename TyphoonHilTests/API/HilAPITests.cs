using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using TyphoonHil.API;

namespace TyphoonHilTests.API
{
    [TestClass()]
    public class HilAPITests
    {
        [TestMethod()]
        public void SetScadaInputValueTest()
        {
            var p = new JObject() { {"result", null }};
            double? p2 = (double?)p["result"]!;
        }
    }
}