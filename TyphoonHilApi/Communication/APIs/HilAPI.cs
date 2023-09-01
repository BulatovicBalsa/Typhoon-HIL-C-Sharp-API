using Newtonsoft.Json.Linq;
using TyphoonHilApi.Communication.Exceptions;

namespace TyphoonHilApi.Communication.APIs
{
    internal class HilAPI : AbsractAPI
    {
        public override int ProperPort => Ports.HilApiPort;
        public HilAPI() { }

        public HilAPI(ICommunication communication) : base(communication) { }

        protected override JObject HandleRequest(string method, JObject parameters)
        {
            {
                var res = Request(method, parameters);
                if (res.ContainsKey("error"))
                {
                    var msg = (string)res["error"]!["message"]!;
                    throw new HilAPIException(msg);
                }

                return res;
            }
        }

        public bool LoadModel(string file = "", bool offlineMode = false, bool vhilDevice = false)
        {
            var parameters = new JObject()
            {
                { "file", file },
                { "offline_mode", offlineMode },
                { "vhil_device", vhilDevice },
            };

            return (bool)HandleRequest("load_model", parameters)["result"]!;
        }

        public bool LoadSettingsFile(string file = "")
        {
            var parameters = new JObject()
            {
                { "file", file },
            };

            return (bool)HandleRequest("load_settings_file", parameters)["result"]!;
        }

        public bool SaveSettingsFile(string filePath)
        {
            var parameters = new JObject()
            {
                { "filePath", filePath },
            };

            return (bool)HandleRequest("save_settings_file", parameters)["result"]!;
        }

        public bool SaveModelState(string saveTo)
        {
            var parameters = new JObject()
            {
                { "save_to", saveTo },
            };

            return (bool)HandleRequest("save_model_state", parameters)["result"]!;
        }

        public bool UploadStandaloneModel(int modelLocation)
        {
            var parameters = new JObject()
            {
                { "model_location", modelLocation },
            };

            return (bool)HandleRequest("upload_standalone_model", parameters)["result"]!;
        }

        public bool AddDataLogger(string name, List<string> signals, string dataFile, bool useSuffix = true)
        {
            var parameters = new JObject()
            {
                { "name", name },
                { "signals", new JArray(signals) },
                { "data_file", dataFile },
                { "use_suffix", useSuffix },
            };

            return (bool)HandleRequest("add_data_logger", parameters)["result"]!;
        }

        public bool StopDataLogger(string name)
        {
            var parameters = new JObject()
            {
                { "name", name },
            };

            return (bool)HandleRequest("stop_data_logger", parameters)["result"]!;
        }

        public bool SetSourceArbitraryWaveform(string name, string file)
        {
            var parameters = new JObject()
            {
                { "name", name },
                { "file", file },
            };

            return (bool)HandleRequest("set_source_arbitrary_waveform", parameters)["result"]!;
        }

        public bool SetPeSwitchingBlockControlMode(string blockName = "", string switchName = "", bool swControl = true, double? executeAt = null)
        {
            var parameters = new JObject()
            {
                { "blockName", blockName },
                { "switchName", switchName },
                { "swControl", swControl },
                { "executeAt", executeAt },
            };

            return (bool)HandleRequest("set_pe_switching_block_control_mode", parameters)["result"]!;
        }

        public bool SetPeSwitchingBlockSoftwareValue(string blockName = "", string switchName = "", int value = 0, double? executeAt = null)
        {
            var parameters = new JObject()
            {
                { "blockName", blockName },
                { "switchName", switchName },
                { "value", value },
                { "executeAt", executeAt },
            };

            return (bool)HandleRequest("set_pe_switching_block_software_value", parameters)["result"]!;
        }

        public bool SetAnalogOutput(int channel, string? name = null, double? scaling = null, double? offset = null, int device = 0)
        {
            var parameters = new JObject()
            {
                { "channel", channel },
                { "name", name },
                { "scaling", scaling },
                { "offset", offset },
                { "device", device },
            };

            return (bool)HandleRequest("set_analog_output", parameters)["result"]!;
        }

        public bool SetDigitalOutput(int channel, string? name = null, bool? invert = null, bool? swControl = null, int? value = null, int device = 0)
        {
            var parameters = new JObject()
            {
                { "channel", channel },
                { "name", name },
                { "invert", invert },
                { "swControl", swControl },
                { "value", value },
                { "device", device },
            };

            return (bool)HandleRequest("set_digital_output", parameters)["result"]!;
        }

        public bool SetMachineConstantTorque(string name = "", double value = 0.0, string? executeAt = null)
        {
            var parameters = new JObject()
            {
                { "name", name },
                { "value", value },
                { "executeAt", executeAt },
            };

            return (bool)HandleRequest("set_machine_constant_torque", parameters)["result"]!;
        }

        public bool SetMachineSquareTorque(string name = "", double value = 0.0, string? executeAt = null)
        {
            var parameters = new JObject()
            {
                { "name", name },
                { "value", value },
                { "executeAt", executeAt },
            };

            return (bool)HandleRequest("set_machine_square_torque", parameters)["result"]!;
        }
        public bool SetMachineLinearTorque(string name = "", double value = 0.0, string? executeAt = null)
        {
            var parameters = new JObject()
            {
                { "name", name },
                { "value", value },
                { "executeAt", executeAt },
            };

            return (bool)HandleRequest("set_machine_linear_torque", parameters)["result"]!;
        }
        public bool SetMachineInitialAngle(string name = "", double angle = 0.0)
        {
            var parameters = new JObject()
            {
                { "name", name },
                { "angle", angle },
            };

            return (bool)HandleRequest("set_machine_initial_angle", parameters)["result"]!;
        }
        public bool SetMachineInitialSpeed(string name = "", double speed = 0.0)
        {
            var parameters = new JObject()
            {
                { "name", name },
                { "speed", speed },
            };

            return (bool)HandleRequest("set_machine_initial_speed", parameters)["result"]!;
        }

        public void SetMachineIncEncoderOffset(string name = "", double offset = 0.0)
        {
            var parameters = new JObject()
            {
                { "name", name },
                { "offset", offset },
            };
            HandleRequest("set_machine_inc_encoder_offset", parameters);
        }

        public void SetMachineSinEncoderOffset(string name = "", double offset = 0.0)
        {
            var parameters = new JObject()
            {
                { "name", name },
                { "offset", offset },
            };

            HandleRequest("set_machine_sin_encoder_offset", parameters);
        }

        public bool StartSimulation()
        {
            var result = HandleRequest("start_simulation");

            return (bool)result["result"]!;
        }

        public bool StartCapture(List<object> cpSettings, List<object> trSettings, List<string> chSettings, List<object> dataBuffer, string fileName = "", string? executeAt = null, string? timeout = null)
        {
            var parameters = new JObject()
            {
                { "cpSettings", new JArray(cpSettings) },
                { "trSettings", new JArray(trSettings) },
                { "chSettings", new JArray(chSettings) },
                { "dataBuffer", new JArray(dataBuffer) },
                { "fileName", fileName },
                { "executeAt", executeAt },
                { "timeout", timeout },
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
            var parameters = new JObject()
            {
                { "name", name },
                { "frictional", frictional },
            };

            var result = HandleRequest("set_machine_constant_torque_type", parameters);

            return (bool)result["result"]!;
        }

    }
}
