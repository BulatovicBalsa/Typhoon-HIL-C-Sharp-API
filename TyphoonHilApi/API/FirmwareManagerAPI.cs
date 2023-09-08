using Newtonsoft.Json.Linq;
using TyphoonHilApi.Communication.Exceptions;

namespace TyphoonHilApi.API;

public class HilDeviceInfo
{
    public int DeviceId { get; set; }
    public string SerialNumber { get; set; }
    public int ConfigurationId { get; set; }
    public string ProductName { get; set; }
    public string FirmwareReleaseDate { get; set; }
    public string CalibrationDate { get; set; }

    public HilDeviceInfo(JObject deviceInfo)
    {
        DeviceId = (int)deviceInfo["device_id"]!;
        SerialNumber = (string)deviceInfo["serial_number"]!;
        ConfigurationId = (int)deviceInfo["configuration_info"]!;
        ProductName = (string)deviceInfo["product_name"]!;
        FirmwareReleaseDate = (string)deviceInfo["firmware_release_date"]!;
        CalibrationDate = (string)deviceInfo["calibration_date"]!;
    }
}

public class FirmwareManagerAPI : AbstractAPI
{
    public override int ProperPort => Ports.FwApiPort;

    protected override JObject HandleRequest(string method, JObject parameters)
    {
        var res = Request(method, parameters);
        if (!res.ContainsKey("error")) return res;
        var msg = (string)res["error"]!["message"]!;
        throw new FirmwareManagerAPIException(msg);
    }

    public List<HilDeviceInfo>? GetHilInfo()
    {
        var result = HandleRequest("get_hil_info")["result"]!;
        return result.Value<JArray?>()?.Select(deviceInfo => new HilDeviceInfo((JObject)deviceInfo)).ToList();
    }

    public void UpdateFirmware(int deviceId = 0, int? configurationId = null, bool force = false)
    {
        var parameters = new JObject()
        {
            { "device_id", deviceId },
            { "configuration_id", configurationId },
            { "force", force },
        };

        HandleRequest("update_firmware", parameters);
    }

}