using Microsoft.VisualStudio.TestTools.UnitTesting;
using TyphoonHil.API;

namespace TyphoonHilTests.API;

[TestClass]
public class DeviceManagerAPITests
{
    [TestMethod]
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