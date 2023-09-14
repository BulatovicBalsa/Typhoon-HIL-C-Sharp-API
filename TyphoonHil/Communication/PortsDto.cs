namespace TyphoonHil.Communication;

internal class PortsDto
{
    public int SchematicApiPort { get; set; }
    public int HilApiPort { get; set; }
    public int ScadaApiPort { get; set; }
    public int PvGenApiPort { get; set; }
    public int FwApiPort { get; set; }
    public int ConfigurationManagerApiPort { get; set; }
    public int EnvApiPort { get; set;}
    public int DeviceManagerApiPort { get; set;}
    public int PackageManagerApiPort { get; set; }
    public int ModbusApiPort { get; set; } = 502;
}