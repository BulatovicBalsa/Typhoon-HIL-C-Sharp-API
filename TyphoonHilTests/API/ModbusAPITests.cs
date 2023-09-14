using Microsoft.VisualStudio.TestTools.UnitTesting;
using TyphoonHil.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TyphoonHil.API.Tests
{
    [TestClass()]
    public class ModbusAPITests
    {
        [TestMethod()]
        public void ModbusAPITest()
        {
            new ModbusAPI();
        }
    }
}