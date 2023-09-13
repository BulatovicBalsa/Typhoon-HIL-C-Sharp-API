using Newtonsoft.Json.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Xml.Linq;
using System;
using TyphoonHil.Communication;
using TyphoonHil.Exceptions;

namespace TyphoonHil.API;

public class PvAmbRes
{
    public bool Status { get; set; }
    public double MaxPowerCurrent { get; set; }
    public double MaxPowerVoltage { get; set; }

    internal PvAmbRes(JArray res)
    {
        Status = (bool)res[0]!;
        MaxPowerCurrent = (double)res[1]!;
        MaxPowerVoltage = (double)res[2]!;
    }

    public PvAmbRes()
    {
        
    }
}

public class HwInfo
{
    public string ProductId { get; set; }
    public int DeviceId { get; set; }
    public int ConfigurationId { get; set; }
    public string FirmwareRelease { get; set; }

    internal HwInfo(JArray res)
    {
        ProductId = (string)res[0]!;
        DeviceId = (int)res[1];
        ConfigurationId = (int)res[2];
        FirmwareRelease = (string)res[3]!;
    }

    public HwInfo()
    {
        
    }
}

public class Harmonic
{
    public int Number { get; set; }
    public double Rms { get; set; }
    public int Phase { get; set; }

    internal JArray JArray => new(){ Number, Rms, Phase };

    public Harmonic(int number, double rms, int phase)
    {
        Number = number;
        Rms = rms;
        Phase = phase;
    }

    public Harmonic()
    {
    }
}

public enum BootOption
{
    DisableStandaloneBoot = 1,
    UsingSelectedModel = 2,
    UsingModelSelectedByDigitalInputs = 3
}

public class HilAPI : AbstractAPI
{
    public const string RmEmbedded = "RM_EMBEDDED";
    public const string RmSystem = "RM_SYSTEM";

    public const string FlArithmeticOverflow = "FL_ARITHMETIC_OVERFLOW";
    public const string FlDeadTime = "FL_DEAD_TIME";
    public const string FlSpExcOccurred = "FL_SP_EXC_OCCURRED";
    public const string FlCompIntOverrun = "FL_COMP_INT_OVERRUN";
    public HilAPI()
    {
    }

    internal HilAPI(ICommunication communication) : base(communication)
    {
    }

    protected override int ProperPort => Ports.HilApiPort;

    protected override JObject HandleRequest(string method, JObject parameters)
    {
        {
            var res = Request(method, parameters);
            if (!res.ContainsKey("error")) return res;
            var msg = (string)res["error"]!["message"]!;
            throw new HilAPIException(msg);
        }
    }

    public bool LoadModel(string file = "", bool offlineMode = false, bool vhilDevice = false)
    {
        var parameters = new JObject
        {
            { "file", file },
            { "offlineMode", offlineMode },
            { "vhil_device", vhilDevice }
        };

        return (bool)HandleRequest("load_model", parameters)["result"]!;
    }

    public bool LoadSettingsFile(string file = "")
    {
        var parameters = new JObject
        {
            { "file", file }
        };

        return (bool)HandleRequest("load_settings_file", parameters)["result"]!;
    }

    public bool SaveSettingsFile(string filePath)
    {
        var parameters = new JObject
        {
            { "filePath", filePath }
        };

        return (bool)HandleRequest("save_settings_file", parameters)["result"]!;
    }

    public bool SaveModelState(string saveTo)
    {
        var parameters = new JObject
        {
            { "save_to", saveTo }
        };

        return (bool)HandleRequest("save_model_state", parameters)["result"]!;
    }

    public bool UploadStandaloneModel(int modelLocation)
    {
        var parameters = new JObject
        {
            { "model_location", modelLocation }
        };

        return (bool)HandleRequest("upload_standalone_model", parameters)["result"]!;
    }

    public bool AddDataLogger(string name, List<string> signals, string dataFile, bool useSuffix = true)
    {
        var parameters = new JObject
        {
            { "name", name },
            { "signals", new JArray(signals) },
            { "data_file", dataFile },
            { "use_suffix", useSuffix }
        };

        return (bool)HandleRequest("add_data_logger", parameters)["result"]!;
    }

    public bool StopDataLogger(List<string> names)
    {
        var parameters = new JObject
        {
            { "name", new JArray(names) }
        };

        return (bool)HandleRequest("stop_data_logger", parameters)["result"]!;
    }

    public bool SetSourceArbitraryWaveform(List<string> names, List<string> files)
    {
        var parameters = new JObject
        {
            { "name", new JArray(names) },
            { "file", new JArray(files) }
        };

        return (bool)HandleRequest("set_source_arbitrary_waveform", parameters)["result"]!;
    }

    public bool SetSourceArbitraryWaveform(string name, string file)
    {
        var parameters = new JObject
        {
            { "name", name },
            { "file", file }
        };

        return (bool)HandleRequest("set_source_arbitrary_waveform", parameters)["result"]!;
    }

    public bool SetPeSwitchingBlockControlMode(string blockName = "", string switchName = "", bool swControl = true,
        double? executeAt = null)
    {
        var parameters = new JObject
        {
            { "blockName", blockName },
            { "switchName", switchName },
            { "swControl", swControl },
            { "executeAt", executeAt }
        };

        return (bool)HandleRequest("set_pe_switching_block_control_mode", parameters)["result"]!;
    }

    public bool SetPeSwitchingBlockSoftwareValue(string blockName = "", string switchName = "", int value = 0,
        double? executeAt = null)
    {
        var parameters = new JObject
        {
            { "blockName", blockName },
            { "switchName", switchName },
            { "value", value },
            { "executeAt", executeAt }
        };

        return (bool)HandleRequest("set_pe_switching_block_software_value", parameters)["result"]!;
    }

    public bool SetAnalogOutput(int channel, string? name = null, double? scaling = null, double? offset = null,
        int device = 0)
    {
        var parameters = new JObject
        {
            { "channel", channel },
            { "name", name },
            { "scaling", scaling },
            { "offset", offset },
            { "device", device }
        };

        return (bool)HandleRequest("set_analog_output", parameters)["result"]!;
    }

    public bool SetDigitalOutput(int channel, string? name = null, bool? invert = null, bool? swControl = null,
        int? value = null, int device = 0)
    {
        var parameters = new JObject
        {
            { "channel", channel },
            { "name", name },
            { "invert", invert },
            { "swControl", swControl },
            { "value", value },
            { "device", device }
        };

        return (bool)HandleRequest("set_digital_output", parameters)["result"]!;
    }

    public bool SetMachineConstantTorque(string name = "", double value = 0.0, double? executeAt = null)
    {
        var parameters = new JObject
        {
            { "name", name },
            { "value", value },
            { "executeAt", executeAt }
        };

        return (bool)HandleRequest("set_machine_constant_torque", parameters)["result"]!;
    }

    public bool SetMachineSquareTorque(string name = "", double value = 0.0, double? executeAt = null)
    {
        var parameters = new JObject
        {
            { "name", name },
            { "value", value },
            { "executeAt", executeAt }
        };

        return (bool)HandleRequest("set_machine_square_torque", parameters)["result"]!;
    }

    public bool SetMachineLinearTorque(string name = "", double value = 0.0, double? executeAt = null)
    {
        var parameters = new JObject
        {
            { "name", name },
            { "value", value },
            { "executeAt", executeAt }
        };

        return (bool)HandleRequest("set_machine_linear_torque", parameters)["result"]!;
    }

    public bool SetMachineInitialAngle(string name = "", double angle = 0.0)
    {
        var parameters = new JObject
        {
            { "name", name },
            { "angle", angle }
        };

        return (bool)HandleRequest("set_machine_initial_angle", parameters)["result"]!;
    }

    public bool SetMachineInitialSpeed(string name = "", double speed = 0.0)
    {
        var parameters = new JObject
        {
            { "name", name },
            { "speed", speed }
        };

        return (bool)HandleRequest("set_machine_initial_speed", parameters)["result"]!;
    }

    public bool SetMachineIncEncoderOffset(string name = "", double offset = 0.0)
    {
        var parameters = new JObject
        {
            { "name", name },
            { "offset", offset }
        };
        return (bool)HandleRequest("set_machine_inc_encoder_offset", parameters)["result"]!;
    }

    public bool SetMachineSinEncoderOffset(string name = "", double offset = 0.0)
    {
        var parameters = new JObject
        {
            { "name", name },
            { "offset", offset }
        };

        return (bool)HandleRequest("set_machine_sin_encoder_offset", parameters)["result"]!;
    }

    public bool StartSimulation()
    {
        var result = HandleRequest("start_simulation");

        return (bool)result["result"]!;
    }

    public bool StartCapture(List<object> cpSettings, List<object> trSettings, List<string> chSettings,
        List<object> dataBuffer, string fileName = "", double? executeAt = null, double? timeout = null)
    {
        var parameters = new JObject
        {
            { "cpSettings", new JArray(cpSettings) },
            { "trSettings", new JArray(trSettings) },
            { "chSettings", new JArray(chSettings) },
            { "dataBuffer", new JArray(dataBuffer) },
            { "fileName", fileName },
            { "executeAt", executeAt },
            { "timeout", timeout }
        };

        var result = HandleRequest("start_capture", parameters);

        return (bool)result["result"]!;
    }

    public bool CaptureInProgress()
    {
        var result = HandleRequest("capture_in_progress");

        return (bool)result["in_progress"]!;
    }

    public bool StopSimulation()
    {
        return (bool)HandleRequest("stop_simulation")!;
    }

    public bool EndScriptByUser()
    {
        return (bool)HandleRequest("end_script_by_user")!;
    }

    public bool SetMachineConstantTorqueType(string name = "", bool frictional = true)
    {
        var parameters = new JObject
        {
            { "name", name },
            { "frictional", frictional }
        };

        var result = HandleRequest("set_machine_constant_torque_type", parameters);

        return (bool)result["result"]!;
    }

    // ====================================================

    public bool LoadModelState(string loadFrom)
    {
        var parameters = new JObject
    {
        { "load_from", loadFrom },
    };

        return (bool)HandleRequest("load_model_state", parameters)["result"]!;
    }

    public bool ModelWrite(string modelVariable, List<double> newValues)
    {
        var parameters = new JObject
        {
            { "model_variable", modelVariable },
            { "new_value", new JArray(newValues) },
        };

        return (bool)HandleRequest("model_write", parameters)["result"]!;
    }

    public bool ModelWrite(string modelVariable, double newValue)
    {
        var parameters = new JObject
    {
        { "model_variable", modelVariable },
        { "new_value", newValue },
    };

        return (bool)HandleRequest("model_write", parameters)["result"]!;
    }

    public double ModelRead(string modelVariable)
    {
        var parameters = new JObject
        {
        { "model_variable", modelVariable },
    };

        return (double)HandleRequest("model_read", parameters)["result"]!;
    }

    public bool RemoveDataLogger(List<string> names)
    {
        var parameters = new JObject
    {
        { "name", new JArray(names) },
    };

        return (bool)HandleRequest("remove_data_logger", parameters)["result"]!;
    }

    public bool StartDataLogger(List<string> names)
    {
        var parameters = new JObject
    {
        { "name", new JArray(names)},
    };

        return (bool)HandleRequest("start_data_logger", parameters)["result"]!;
    }

    public bool UpdateSources(List<string> sources, double? executeAt= null)
    {
        var parameters = new JObject
        {
            { "sources", new JArray(sources) },
        { "executeAt", executeAt },
    };

        return (bool)HandleRequest("update_sources", parameters)["result"]!;
    }

    public bool PrepareSourceArbitraryWaveform(List<string> names, List<string>? files = null)
    {
        var parameters = new JObject
        {
            { "name", new JArray(names) },
            { "file", files is null ? "" : new JArray(files) },
        };

        return (bool)HandleRequest("prepare_source_arbitrary_waveform", parameters)["result"]!;
    }

    public bool PrepareSourceArbitraryWaveform(string name, string file= "")
    {
        var parameters = new JObject
        {
            { "name", name },
        { "file", file },
    };

        return (bool)HandleRequest("prepare_source_arbitrary_waveform", parameters)["result"]!;
    }

    public bool PrepareSourceConstantValue(List<string> names, List<double>? values = null)
    {
        var parameters = new JObject
        {
            { "name", new JArray(names) },
            { "value", values is null ? 0 : new JArray(values) },
        };

        return (bool)HandleRequest("prepare_source_constant_value", parameters)["result"]!;
    }

    public bool PrepareSourceConstantValue(string name, double value= 0)
    {
        var parameters = new JObject
        {
            { "name", name },
            { "value", value },
        };

        return (bool)HandleRequest("prepare_source_constant_value", parameters)["result"]!;
    }

    public bool PrepareSourceSineWaveform(List<string> names, List<double>? rms = null, List<double>? frequency = null, List<double>? phase = null, List<Harmonic>? harmonics = null, List<Harmonic>? harmonicsPu = null)
    {
        var parameters = new JObject
    {
        { "name", new JArray(names) },
        { "rms", rms is null ? null : new JArray(rms) },
        { "frequency", frequency is null ? null : new JArray(frequency) },
        { "phase", phase is null ? null : new JArray() },
        { "harmonics", harmonics is null ? null : new JArray(harmonics.Select(item => item.JArray)) },
        { "harmonics_pu", harmonicsPu is null ? null : new JArray(harmonicsPu.Select(item => item.JArray)) },
    };

        return (bool)HandleRequest("prepare_source_sine_waveform", parameters)["result"]!;
    }

    public bool EnableAoLimiting(int channel, double lowerLimit, double upperLimit, int device= 0)
    {
        var parameters = new JObject
    {
        { "channel", channel },
        { "lower_limit", lowerLimit },
        { "upper_limit", upperLimit },
        { "device", device },
        };

        return (bool)HandleRequest("enable_ao_limiting", parameters)["result"]!;
    }

    public bool DisableAoLimiting(int channel, int device= 0)
    {
        var parameters = new JObject
    {
        { "channel", channel },
        { "device", device },
    };

        return (bool)HandleRequest("disable_ao_limiting", parameters)["result"]!;
    }

    public bool SetBootConfiguration(BootOption bootOpt, int? modelLocation= null, List<int>? digitalSettings= null)
    {
        var parameters = new JObject
    {
        { "boot_opt", (int) bootOpt },
        { "model_location", modelLocation },
        { "digital_settings", digitalSettings is null ? null : new JArray(digitalSettings) },
    };

        return (bool)HandleRequest("set_boot_configuration", parameters)["result"]!;
    }

    public bool SetSourceConstantValue(List<string> names, List<double>? values=null, double? executeAt = null, int rampTime = 0, string rampType = "lin")
    {
        var parameters = new JObject
        {
            { "name", new JArray(names) },
            { "value", values is null ? 0 : new JArray(values)},
            { "executeAt", executeAt },
            { "ramp_time", rampTime },
            { "ramp_type", rampType },
        };

        return (bool)HandleRequest("set_source_constant_value", parameters)["result"]!;
    }

    public bool SetSourceConstantValue(string name, double value= 0, double? executeAt= null, int rampTime= 0, string rampType= "lin")
    {
        var parameters = new JObject
    {
        { "name", name },
        { "value", value },
            { "executeAt", executeAt },
            { "ramp_time", rampTime },
            { "ramp_type", rampType },
        };

        return (bool)HandleRequest("set_source_constant_value", parameters)["result"]!;
    }

    public bool SetSourceSineWaveform(List<string> names, List<double>? rms= null, List<double>? frequency= null, List<double>? phase= null, List<Harmonic>? harmonics= null, List<Harmonic>? harmonicsPu= null, double? executeAt= null, int rampTime= 0, string rampType= "lin")
    {
        var parameters = new JObject
    {
        { "name", new JArray(names) },
        { "rms", rms is null ? null : new JArray(rms) },
        { "frequency", frequency is null ? null : new JArray(frequency) },
        { "phase", phase is null ? null : new JArray() },
        { "harmonics", harmonics is null ? null : new JArray(harmonics.Select(item => item.JArray)) },
        { "harmonics_pu", harmonicsPu is null ? null : new JArray(harmonicsPu.Select(item => item.JArray)) },
            { "executeAt", executeAt },
            { "ramp_time", rampTime },
        { "ramp_type", rampType },
    };

        return (bool)HandleRequest("set_source_sine_waveform", parameters)["result"]!;
    }

    public bool SetSourceScaling(List<string> name, List<double> scaling, double? executeAt = null, int rampTime = 0, string rampType = "lin")
    {
        var parameters = new JObject
        {
            { "name", new JArray(name) },
            { "scaling", new JArray(scaling) },
            { "executeAt", executeAt },
            { "ramp_time", rampTime },
            { "ramp_type", rampType },
        };

        return (bool)HandleRequest("set_source_scaling", parameters)["result"]!;
    }

    public bool SetSourceScaling(string name, double scaling, double? executeAt= null, int rampTime= 0, string rampType= "lin")
    {
        var parameters = new JObject
    {
        { "name", name },
            { "scaling", scaling },
            { "executeAt", executeAt },
        { "ramp_time", rampTime },
        { "ramp_type", rampType },
        };

        return (bool)HandleRequest("set_source_scaling", parameters)["result"]!;
    }

    public bool SetPvInputFile(string name, string file, double illumination= 0.0, double temperature= 0.0, double isc= 10.0, double voc= 100.0)
    {
        var parameters = new JObject
    {
        { "name", name },
            { "file", file },
            { "illumination", illumination },
            { "temperature", temperature },
            { "isc", isc },
        { "voc", voc },
    };

        return (bool)HandleRequest("set_pv_input_file", parameters)["result"]!;
    }

    public PvAmbRes SetPvAmbParams(string name, double? illumination= null, double? temperature= null, double? isc= null, double? voc= null, double? executeAt= null, double? rampTime= 0, string rampType= "lin")
    {
        var parameters = new JObject
    {
        { "name", name },
        { "illumination", illumination },
        { "temperature", temperature },
            { "isc", isc },
            { "voc", voc },
            { "executeAt", executeAt },
        { "ramp_time", rampTime },
        { "ramp_type", rampType },
    };

        return new PvAmbRes((JArray)HandleRequest("set_pv_amb_params", parameters)["result"]!);
    }

    public bool SetAnalogOutputSignal(int channel, string name, int device= 0)
    {
        var parameters = new JObject
    {
        { "channel", channel },
        { "name", name },
        { "device", device },
    };

        return (bool)HandleRequest("set_analog_output_signal", parameters)["result"]!;
    }

    public bool SetAnalogOutputScaling(int channel, double scaling= 0.0, int device= 0)
    {
        var parameters = new JObject
    {
        { "channel", channel },
        { "scaling", scaling },
        { "device", device },
    };

        return (bool)HandleRequest("set_analog_output_scaling", parameters)["result"]!;
    }

    public bool SetAnalogOutputOffset(int channel, double offset= 0.0, int device= 0)
    {
        var parameters = new JObject
        {
            { "channel", channel },
        { "offset", offset },
        { "device", device },
    };

        return (bool)HandleRequest("set_analog_output_offset", parameters)["result"]!;
    }

    public bool SetDigitalOutputSignal(int channel, string name, int device= 0)
    {
        var parameters = new JObject
        {
            { "channel", channel },
            { "name", name },
        { "device", device },
    };

        return (bool)HandleRequest("set_digital_output_signal", parameters)["result"]!;
    }

    public bool SetDigitalOutputInverting(int channel, bool invert= false, int device= 0)
    {
        var parameters = new JObject
    {
        { "channel", channel },
        { "invert", invert },
        { "device", device },
    };

        return (bool)HandleRequest("set_digital_output_inverting", parameters)["result"]!;
    }

    public bool SetDigitalOutputSwControl(int channel, bool swControl= false, int device= 0)
    {
        var parameters = new JObject
    {
        { "channel", channel },
        { "swControl", swControl },
        { "device", device },
    };

        return (bool)HandleRequest("set_digital_output_sw_control", parameters)["result"]!;
    }

    public bool SetDigitalOutputSoftwareValue(int channel, int value= 0, int device= 0)
    {
        var parameters = new JObject
    {
        { "channel", channel },
        { "value", value },
        { "device", device },
    };

        return (bool)HandleRequest("set_digital_output_software_value", parameters)["result"]!;
    }

    public bool SetContactor(string name, bool? swControl= null, bool? swState= null, double? executeAt= null)
    {
        var parameters = new JObject
        {
        { "name", name },
        { "swControl", swControl },
        { "swState", swState },
        { "executeAt", executeAt },
    };

        return (bool)HandleRequest("set_contactor", parameters)["result"]!;
    }

    public bool SetContactorControlMode(string name, bool swControl= false, double? executeAt= null)
    {
        var parameters = new JObject
    {
        { "name", name },
        { "swControl", swControl },
        { "executeAt", executeAt },
    };

        return (bool)HandleRequest("set_contactor_control_mode", parameters)["result"]!;
    }

    public bool SetContactorState(string name, bool swState= false, double? executeAt= null)
    {
        var parameters = new JObject
        {
        { "name", name },
        { "swState", swState },
        { "executeAt", executeAt },
    };

        return (bool)HandleRequest("set_contactor_state", parameters)["result"]!;
    }

    public bool SetMachineLoadSource(string name= "", bool software= true)
    {
        var parameters = new JObject
    {
        { "name", name },
        { "software", software },
    };

        return (bool)HandleRequest("set_machine_load_source", parameters)["result"]!;
    }

    public bool SetMachineExternalTorqueType(string name= "", bool frictional= true)
    {
        var parameters = new JObject
    {
        { "name", name },
        { "frictional", frictional },
    };

        return (bool)HandleRequest("set_machine_external_torque_type", parameters)["result"]!;
    }

    public bool SetMachineLoadType(string name= "", bool torque= true)
    {
        var parameters = new JObject
    {
        { "name", name },
        { "torque", torque },
    };

        return (bool)HandleRequest("set_machine_load_type", parameters)["result"]!;
    }

    public bool SetMachineSpeed(string name= "", double speed= 0.0, double? executeAt= null)
    {
        var parameters = new JObject
        {
        { "name", name },
        { "speed", speed },
        { "executeAt", executeAt },
    };

        return (bool)HandleRequest("set_machine_speed", parameters)["result"]!;
    }

    public bool SetMachineEncoderOffset(string name= "", double offset= 0.0)
    {
        var parameters = new JObject
    {
        { "name", name },
        { "offset", offset },
    };

        return (bool)HandleRequest("set_machine_encoder_offset", parameters)["result"]!;
    }

    public bool SetMachineResolverOffset(string name= "", double offset= 0.0)
    {
        var parameters = new JObject
    {
        { "name", name },
        { "offset", offset },
    };

        return (bool)HandleRequest("set_machine_resolver_offset", parameters)["result"]!;
    }

    public bool SetInitialBatterySoc(string batteryName, double initialValue)
    {
        var parameters = new JObject
    {
        { "batteryName", batteryName },
        { "initialValue", initialValue },
    };

        return (bool)HandleRequest("set_initial_battery_soc", parameters)["result"]!;
    }

    public bool SetScadaInputValue(string scadaInputName, double value)
    {
        var parameters = new JObject
    {
        { "scadaInputName", scadaInputName },
        { "value", value },
    };

        return (bool)HandleRequest("set_scada_input_value", parameters)["result"]!;
    }

    public bool SetCpInputValue(string cpCategory, string cpGroup, string cpInputName, double value)
    {
        var parameters = new JObject
    {
        { "cpCategory", cpCategory },
        { "cpGroup", cpGroup },
        { "cpInputName", cpInputName },
        { "value", value },
    };

        return (bool)HandleRequest("set_cp_input_value", parameters)["result"]!;
    }

    public void SetTextMode(string mode)
    {
        var parameters = new JObject
    {
        { "mode", mode },
    };

        HandleRequest("set_text_mode", parameters);
    }

    public void SetDebugLevel(int level= 0)
    {
        var parameters = new JObject
    {
        { "level", level },
    };

        HandleRequest("set_debug_level", parameters);
    }

    public bool StopCapture()
    {
        var parameters = new JObject();

        return (bool)HandleRequest("stop_capture", parameters)["result"]!;
    }

    public bool IsSimulationRunning()
    {
        var parameters = new JObject();

        return (bool)HandleRequest("is_simulation_running", parameters)["result"]!;
    }

    public bool CheckHilHwid()
    {
        var parameters = new JObject();

        return (bool)HandleRequest("check_hil_hwid", parameters)["result"]!;
    }

    public bool TimeoutOccurred()
    {
        var parameters = new JObject();

        return (bool)HandleRequest("timeout_occurred", parameters)["result"]!;
    }

    public PvAmbRes ReadPvIvCurve(string name, double voltage)
    {
        var parameters = new JObject
    {
        { "name", name },
        { "voltage", voltage },
    };

        return new PvAmbRes((JArray)HandleRequest("read_pv_iv_curve", parameters)["result"]!);
    }

    public double? ReadAnalogSignal(string name= "")
    {
        var parameters = new JObject
    {
        { "name", name },
    };

        return (double?)HandleRequest("read_analog_signal", parameters)["result"]!;
    }

    public JArray ReadAnalogSignals(List<string>? signals=null)
    {
        var parameters = new JObject
    {
        { "signals", signals is null ? new JArray() : new JArray(signals) },
    };

        return (JArray?)HandleRequest("read_analog_signals", parameters)["result"]!;
    }

    public double? ReadDigitalSignal(string name= "", int? device= null)
    {
        var parameters = new JObject
    {
        { "name", name },
        { "device", device },
    };

        return (double?)HandleRequest("read_digital_signal", parameters)["result"]!;
    }

    public JArray ReadDigitalSignals(List<string>? signals = null)
    {
        var parameters = new JObject
    {
        { "signals", new JArray(signals??new List<string>()) },
    };
        
        return (JArray?)HandleRequest("read_digital_signals", parameters)["result"]!;
    }

    public int? ReadDigitalInput(int pinNum= 1, int device= 0)
    {
        var parameters = new JObject
    {
        { "pinNum", pinNum },
        { "device", device },
    };

        return (int?)HandleRequest("read_digital_input", parameters)["result"]!;
    }

    public JObject ReadStreamingSignals(List<string> signals, int fromLastIndex= 0)
    {
        var parameters = new JObject
    {
        { "signals", new JArray(signals) },
        { "from_last_index", fromLastIndex },
    };

        return (JObject)HandleRequest("read_streaming_signals", parameters)["result"]!;
    }

    public JObject LoadSignalGenData(string filename)
    {
        var parameters = new JObject
    {
        { "filename", filename },
    };

        return (JObject)HandleRequest("load_signal_gen_data", parameters)["result"]!;
    }

    public JObject CreateSignalStimulus(JObject signalData)
    {
        var parameters = new JObject
    {
        { "signal_data", signalData },
    };

        return (JObject)HandleRequest("create_signal_stimulus", parameters)["result"]!;
    }

    public bool PrepareSignalStimulus(JObject signalStimulus)
    {
        var parameters = new JObject
    {
        { "signal_stimulus", signalStimulus },
    };

        return (bool)HandleRequest("prepare_signal_stimulus", parameters)["result"]!;
    }

    public bool StartSignalStimulus(JObject signalStimulus)
    {
        var parameters = new JObject
    {
        { "signal_stimulus", signalStimulus },
    };

        return (bool)HandleRequest("start_signal_stimulus", parameters)["result"]!;
    }

    public bool StopSignalStimulus(JObject signalStimulus)
    {
        var parameters = new JObject
    {
        { "signal_stimulus", signalStimulus },
    };

        return (bool)HandleRequest("stop_signal_stimulus", parameters)["result"]!;
    }

    public bool PauseSignalStimulus(JObject signalStimulus)
    {
        var parameters = new JObject
    {
        { "signal_stimulus", signalStimulus },
    };

        return (bool)HandleRequest("pause_signal_stimulus", parameters)["result"]!;
    }

    public bool RebootHil()
    {
        var parameters = new JObject();

        return (bool)HandleRequest("reboot_hil", parameters)["result"]!;
    }

    public bool WaitSec(int sec)
    {
        var parameters = new JObject
    {
        { "sec", sec },
    };

        return (bool)HandleRequest("wait_sec", parameters)["result"]!;
    }

    public bool WaitMsec(int msec)
    {
        var parameters = new JObject
    {
        { "msec", msec },
    };

        return (bool)HandleRequest("wait_msec", parameters)["result"]!;
    }

    public bool WaitOnUser()
    {
        var parameters = new JObject();

        return (bool)HandleRequest("wait_on_user", parameters)["result"]!;
    }

    public bool ResetFlagStatus(string flag, int device = 0)
    {
        var parameters = new JObject
    {
        { "flag", flag },
        { "device", device },
    };

        return (bool)HandleRequest("reset_flag_status", parameters)["result"]!;
    }

    public JObject GetModelVariables(string nameSeparator= ".")
    {
        var parameters = new JObject
        {
        { "name_separator", nameSeparator },
    };

        return (JObject)HandleRequest("get_model_variables", parameters)["result"]!;
    }

    public string? GetCpOutputValue(string cpCategory, string cpGroup, string cpOutputName)
    {
        var parameters = new JObject
    {
        { "cpCategory", cpCategory },
        { "cpGroup", cpGroup },
        { "cpOutputName", cpOutputName },
    };

        return (string?)HandleRequest("get_cp_output_value", parameters)["result"]!;
    }

    public string? GetScadaOutputValue(string scadaOutputName)
    {
        var parameters = new JObject
    {
        { "scadaOutputName", scadaOutputName },
    };

        return (string?)HandleRequest("get_scada_output_value", parameters)["result"]!;
    }

    public string? GetBatterySoc(string batteryName)
    {
        var parameters = new JObject
    {
        { "batteryName", batteryName },
    };

        return (string?)HandleRequest("get_battery_soc", parameters)["result"]!;
    }

    public PvAmbRes GetPvMpp(string name)
    {
        var parameters = new JObject
    {
        { "name", name },
    };

        return new PvAmbRes((JArray)HandleRequest("get_pv_mpp", parameters)["result"]!);
    }

    public int GetNumOfConnectedHils()
    {
        var parameters = new JObject();

        return (int)HandleRequest("get_num_of_connected_hils", parameters)["result"]!;
    }

    public double? GetSimStep(int device= 0)
    {
        var parameters = new JObject
    {
        { "device", device },
    };

        return (double?)HandleRequest("get_sim_step", parameters)["result"]!;
    }

    public double? GetSimTime(int device= 0)
    {
        var parameters = new JObject
    {
        { "device", device },
    };

        return (double?)HandleRequest("get_sim_time", parameters)["result"]!;
    }

    public JObject GetDeviceCfgList()
    {
        var parameters = new JObject();

        return (JObject)HandleRequest("get_device_cfg_list", parameters)["result"]!;
    }

    public string GetSwVersion()
    {
        var parameters = new JObject();

        return (string)HandleRequest("get_sw_version", parameters)["result"]!;
    }

    public string GetHilCalibrationDate(int deviceId = 0)
    {
        var parameters = new JObject
    {
        { "device_id", deviceId },
    };

        return (string)HandleRequest("get_hil_calibration_date", parameters)["result"]!;
    }

    public JObject? GetDeviceFeatures(string? device= null, int? confId= null, string? feature= null)
    {
        var parameters = new JObject
    {
        { "device", device },
        { "conf_id", confId },
        { "feature", feature },
    };

        return (JObject?)HandleRequest("get_device_features", parameters)["result"]!;
    }

    public HwInfo GetHwInfo()
    {
        var parameters = new JObject();

        return new HwInfo((JArray)HandleRequest("get_hw_info", parameters)["result"]!);
    }

    public bool? GetFlagStatus(string flag, int device= 0)
    {
        var parameters = new JObject
    {
        { "flag", flag },
        { "device", device },
    };

        return (bool?)HandleRequest("get_flag_status", parameters)["result"]!;
    }

    public List<string?> GetSources()
    {
        var parameters = new JObject();

        return ((JArray)HandleRequest("get_sources", parameters)["result"]!).Select(item => (string?)item).ToList();
    }

    public List<string?> GetPvs()
    {
        var parameters = new JObject();

        return ((JArray)HandleRequest("get_pvs", parameters)["result"]!).Select(item => (string?)item).ToList();
    }

    public List<string?> GetAnalogSignals()
    {
        var parameters = new JObject();

        return ((JArray)HandleRequest("get_analog_signals", parameters)["result"]!).Select(item => (string?)item).ToList();
    }

    public List<string?> GetDigitalSignals()
    {
        var parameters = new JObject();

        return ((JArray)HandleRequest("get_digital_signals", parameters)["result"]!).Select(item => (string?)item).ToList();
    }

    public List<string?> GetStreamingAnalogSignals()
    {
        var parameters = new JObject();

        return ((JArray)HandleRequest("get_streaming_analog_signals", parameters)["result"]!).Select(item => (string?)item).ToList();
    }

    public List<string?> GetStreamingDigitalSignals()
    {
        var parameters = new JObject();

        return ((JArray)HandleRequest("get_streaming_digital_signals", parameters)["result"]!).Select(item => (string?)item).ToList();
    }

    public List<string?> GetContactors()
    {
        var parameters = new JObject();

        return ((JArray)HandleRequest("get_contactors", parameters)["result"]!).Select(item => (string?)item).ToList();
    }

    public List<string?> GetMachines()
    {
        var parameters = new JObject();

        return ((JArray)HandleRequest("get_machines", parameters)["result"]!).Select(item => (string?)item).ToList();
    }

    public List<string?> GetPeSwitchingBlocks()
    {
        var parameters = new JObject();

        return ((JArray)HandleRequest("get_pe_switching_blocks", parameters)["result"]!).Select(item => (string?)item).ToList();
    }

    public List<string?> GetScadaInputs()
    {
        var parameters = new JObject();

        return ((JArray)HandleRequest("get_scada_inputs", parameters)["result"]!).Select(item => (string?)item).ToList();
    }

    public List<string?> GetScadaOutputs()
    {
        var parameters = new JObject();

        return ((JArray)HandleRequest("get_scada_outputs", parameters)["result"]!).Select(item => (string?)item).ToList();
    }

    public JObject? GetSourceSettings(string name)
    {
        var parameters = new JObject
    {
        { "name", name },
    };

        return (JObject?)HandleRequest("get_source_settings", parameters)["result"]!;
    }

    public JObject GetPvPanelSettings(string name)
    {
        var parameters = new JObject
    {
        { "name", name },
    };

        return (JObject?)HandleRequest("get_pv_panel_settings", parameters)["result"]!;
    }

    public JObject GetMachineSettings(string name)
    {
        var parameters = new JObject
        {
        { "name", name },
    };

        return (JObject?)HandleRequest("get_machine_settings", parameters)["result"]!;
    }

    public JObject? GetPeSwitchingBlockSettings(string blockName= "", string switchName= "")
    {
        var parameters = new JObject
    {
        { "blockName", blockName },
        { "switchName", switchName },
    };

        return (JObject?)HandleRequest("get_pe_switching_block_settings", parameters)["result"]!;
    }

    public JObject? GetContactorSettings(string name)
    {
        var parameters = new JObject
    {
        { "name", name },
    };

        return (JObject?)HandleRequest("get_contactor_settings", parameters)["result"]!;
    }

    public JObject? GetAnalogOutputSettings(int channel, int device= 0)
    {
        var parameters = new JObject
    {
        { "channel", channel },
        { "device", device },
    };

        return (JObject?)HandleRequest("get_analog_output_settings", parameters)["result"]!;
    }

    public JObject? GetDigitalOutputSettings(int channel, int device= 0)
    {
        var parameters = new JObject
        {
        { "channel", channel },
        { "device", device },
    };

        return (JObject?)HandleRequest("get_digital_output_settings", parameters)["result"]!;
    }

    public JObject GetCpInputSettings(string cpCategory, string cpGroup, string cpInputName)
    {
        var parameters = new JObject
    {
        { "cpCategory", cpCategory },
        { "cpGroup", cpGroup },
        { "cpInputName", cpInputName },
    };

        return (JObject?)HandleRequest("get_cp_input_settings", parameters)["result"]!;
    }

    public JObject? GetScadaInputSettings(string scadaInputName)
    {
        var parameters = new JObject
    {
        { "scadaInputName", scadaInputName },
    };

        return (JObject?)HandleRequest("get_scada_input_settings", parameters)["result"]!;
    }

    public List<int?> GetHilSerialNumber()
    {
        var parameters = new JObject();

        return ((JArray)HandleRequest("get_hil_serial_number", parameters)["result"]!).Select(item => (int?)item).ToList();
    }

    public string? GetNsVar(string varName)
    {
        var parameters = new JObject
    {
            { "var_name", varName },
        };

        return (string?)HandleRequest("get_ns_var", parameters)["result"]!;
    }

    public List<string?> GetNsVars()
    {
        var parameters = new JObject();

        return ((JArray)HandleRequest("get_ns_vars", parameters)["result"]!).Select(item => (string?)item).ToList();
    }

    public JObject GetDataLoggerStatus(string name)
    {
        var parameters = new JObject
    {
        { "name", name },
    };

        return (JObject)HandleRequest("get_data_logger_status", parameters)["result"]!;
    }

    public string GetModelFilePath()
    {
        var parameters = new JObject();

        return (string)HandleRequest("get_model_file_path", parameters)["result"]!;
    }

    public JObject? GetSpMonitorsValues()
    {
        var parameters = new JObject();

        return (JObject?)HandleRequest("get_sp_monitors_values", parameters)["result"]!;
    }

    public bool AvailableSources()
    {
        var parameters = new JObject();

        return (bool)HandleRequest("available_sources", parameters)["result"]!;
    }

    public bool AvailablePvs()
    {
        var parameters = new JObject();

        return (bool)HandleRequest("available_pvs", parameters)["result"]!;
    }

    public bool AvailableAnalogSignals()
    {
        var parameters = new JObject();

        return (bool)HandleRequest("available_analog_signals", parameters)["result"]!;
    }

    public bool AvailableDigitalSignals()
    {
        var parameters = new JObject();

        return (bool)HandleRequest("available_digital_signals", parameters)["result"]!;
    }

    public bool AvailableContactors()
    {
        var parameters = new JObject();

        return (bool)HandleRequest("available_contactors", parameters)["result"]!;
    }

    public bool AvailableMachines()
    {
        var parameters = new JObject();

        return (bool)HandleRequest("available_machines", parameters)["result"]!;
    }

    public bool AvailablePeSwitchingBlocks()
    {
        var parameters = new JObject();

        return (bool)HandleRequest("available_pe_switching_blocks", parameters)["result"]!;
    }


}