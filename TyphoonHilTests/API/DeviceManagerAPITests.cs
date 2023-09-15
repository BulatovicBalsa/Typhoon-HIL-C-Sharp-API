using Microsoft.VisualStudio.TestTools.UnitTesting;
using TyphoonHil.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TyphoonHil.API.Tests
{
    [TestClass()]
    public class DeviceManagerAPITests
    {
        [TestMethod()]
        public void SetDeviceSettingsTest()
        {
            var model = new DeviceManagerAPI();
            Assert.IsTrue(model.AddDevicesToSetup());
            var devices = model.GetDetectedDevices();
            if (devices.Count == 0)
            {
                Console.WriteLine("here");
                Assert.IsFalse(model.IsSetupConnected());
            }

            model.GetSetupDevicesSerials().ForEach(Console.WriteLine);
            Console.WriteLine(model.GetHilInfo());
            Console.WriteLine(model.GetSetupDevices());
            model.GetSetupDevicesSerials().ForEach(Console.WriteLine);
            //Console.WriteLine(model.GetDeviceSettings());
            Assert.IsTrue(model.AddDiscoveryIpAddresses());
            //Assert.IsTrue(model.RemoveDiscoveryIpAddresses());
        }
    }
}