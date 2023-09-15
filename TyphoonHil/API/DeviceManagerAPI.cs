using Newtonsoft.Json.Linq;
using TyphoonHil.Exceptions;

namespace TyphoonHil.API;

public class DeviceManagerAPI : AbstractAPI
{
    protected override int ProperPort => Ports.DeviceManagerApiPort;

    protected override JObject HandleRequest(string method, JObject parameters)
    {
        var res = Request(method, parameters);
        if (!res.ContainsKey("error")) return res;
        var msg = (string)res["error"]!["message"]!;
        throw new DeviceManagerApiException(msg);
    }

    public bool AddDevicesToSetup(List<string>? devices= null)
    {
        var parameters = new JObject
    {
        { "devices", devices is null ? new JArray() : new JArray(devices) },
    };

        return (bool)HandleRequest("add_devices_to_setup", parameters)["result"]!;
    }

    public bool AddDiscoveryIpAddresses(List<string>? addresses= null)
    {
        var parameters = new JObject
    {
        { "addresses", addresses is null ? new JArray() : new JArray(addresses)},
    };

        return (bool)HandleRequest("add_discovery_ip_addresses", parameters)["result"]!;
    }

    public bool ConnectSetup()
    {
        var parameters = new JObject();

        return (bool)HandleRequest("connect_setup", parameters)["result"]!;
    }

    public bool DisconnectSetup()
    {
        var parameters = new JObject();

        return (bool)HandleRequest("disconnect_setup", parameters)["result"]!;
    }

    public JArray GetAvailableDevices()
    {
        var parameters = new JObject();

        return (JArray)HandleRequest("get_available_devices", parameters)["result"]!;
    }

    public JArray GetDetectedDevices()
    {
        var parameters = new JObject();

        return (JArray)HandleRequest("get_detected_devices", parameters)["result"]!;
    }

    public JObject GetDeviceSettings(string deviceSerial)
    {
        var parameters = new JObject
    {
        { "device_serial", deviceSerial },
    };

        return (JObject)HandleRequest("get_device_settings", parameters)["result"]!;
    }

    public JArray? GetHilInfo()
    {
        var parameters = new JObject();

        var res = HandleRequest("get_hil_info", parameters)["result"]!;
        return res.Type == JTokenType.Null ? null : (JArray)res;
    }

    public JArray? GetSetupDevices()
    {
        var parameters = new JObject();

        var res = HandleRequest("get_setup_devices", parameters)["result"]!;
        return res.Type == JTokenType.Null ? null : (JArray)res;
    }

    public List<string> GetSetupDevicesSerials()
    {
        var parameters = new JObject();

        return ((JArray)HandleRequest("get_setup_devices_serials", parameters)["result"]!).Select(item => (string)item!).ToList();
    }

    public bool IsSetupConnected()
    {
        var parameters = new JObject();

        return (bool)HandleRequest("is_setup_connected", parameters)["result"]!;
    }

    public bool LoadSetup(string file= "")
    {
        var parameters = new JObject
    {
        { "file", file },
    };

        return (bool)HandleRequest("load_setup", parameters)["result"]!;
    }

    public bool RemoveDevicesFromSetup(List<string>? devices = null)
    {
        var parameters = new JObject
    {
        { "devices", devices is null ? new JArray() : new JArray(devices) },
    };

        return (bool)HandleRequest("remove_devices_from_setup", parameters)["result"]!;
    }

    public bool RemoveDiscoveryIpAddresses(List<string>? addresses= null)
    {
        var parameters = new JObject
    {
        { "addresses", addresses is null ? new JArray() : new JArray(addresses)},
    };

        return (bool)HandleRequest("remove_discovery_ip_addresses", parameters)["result"]!;
    }

    public void SetDeviceSettings(string deviceSerial, JObject? settings = null) {
        settings ??= new JObject();
	var parameters = new JObject
    {
        { "device_serial", deviceSerial },
        { "settings", settings },
    };

    HandleRequest("set_device_settings", parameters);
    }

public void SyncFirmware(string deviceToUpdate, int? configurationId= null, bool force= false)
{
    var parameters = new JObject
    {
        { "device_to_update", deviceToUpdate },
        { "configuration_id", configurationId },
        { "force", force },
    };

    HandleRequest("sync_firmware", parameters);
}

public void UpdateFirmware(string deviceToUpdate, int? configurationId= null, bool force= false)
{
    var parameters = new JObject
    {
        { "device_to_update", deviceToUpdate },
        { "configuration_id", configurationId },
        { "force", force },
    };

    HandleRequest("update_firmware", parameters);
}


}