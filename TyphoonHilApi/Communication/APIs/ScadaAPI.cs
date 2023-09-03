using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TyphoonHilApi.Communication.Exceptions;

namespace TyphoonHilApi.Communication.APIs
{
    public class ScadaConstants
    {
        // Property constants
        public const string PropName = "name";
        public const string PropHtmlName = "html_name";
        public const string PropNamePosition = "name_position";
        public const string PropLabel = "label";
        public const string PropUseLabel = "use_label";
        public const string PropFqn = "fully_qualified_name";
        public const string PropDescription = "description";
        public const string PropPanelInit = "panel_init_code";
        public const string PropPanelLock = "panel_locked";
        public const string PropPosition = "position";
        public const string PropSize = "size";
        public const string PropAppearance = "appearance";
        public const string PropSignals = "signals";
        public const string PropStreamingSignals = "streaming_signals";
        public const string PropDataType = "data_type";
        public const string PropExpression = "expression_code";
        public const string PropUpdateRate = "update_rate";
        public const string PropTimeWindow = "time_window";
        public const string PropUnit = "unit";
        public const string PropAutoUnit = "auto_unit_assign";
        public const string PropBgColor = "bg_color";
        public const string PropPanelBgColor = "panel_bg_color";
        public const string PropBgType = "bg_type";
        public const string PropUseAsBg = "use_as_bg";
        public const string PropRange = "range";
        public const string PropUseColorRange = "use_color_range";
        public const string PropWarningRange = "warning_range";
        public const string PropCriticalRange = "critical_range";
        public const string PropGreenRange = "green_range";
        public const string PropOrangeRange = "orange_range";
        public const string PropDecimals = "decimals";
        public const string PropScalingFactor = "scaling_factor";
        public const string PropStreamingAnSigScaling = "streaming_analog_signals_scaling";
        public const string PropAnSigScaling = "analog_signals_scaling";
        public const string PropRedRange = "red_range";
        public const string PropLedColor = "led_color";
        public const string PropXTitle = "x_title";
        public const string PropYTitle = "y_title";
        public const string PropXTitleEnabled = "x_title_enabled";
        public const string PropYTitleEnabled = "y_title_enabled";
        public const string PropCustomXTitle = "custom_x_title";
        public const string PropCustomYTitle = "custom_y_title";
        public const string PropCustomXTitleEnabled = "custom_x_title_enabled";
        public const string PropCustomYTitleEnabled = "custom_y_title_enabled";
        public const string PropXRange = "x_range";
        public const string PropYRange = "y_range";
        public const string PropAutoScaleEnabled = "autoscale_enabled";
        public const string PropXAutoScaleEnabled = "x_axis_autoscale_enabled";
        public const string PropYAutoScaleEnabled = "Y_axis_autoscale_enabled";
        public const string PropLegendEnabled = "legend_enabled";
        public const string PropRefCurveEnabled = "ref_curves_enabled";
        public const string PropRefCurve = "ref_curves_code";
        public const string PropPvPanel = "pv_panel";
        public const string PropLineStyle = "line_style";
        public const string PropPlotRange = "plot_range";
        public const string PropPhasorsSettings = "phasors_settings";
        public const string PropBarsSettings = "bars_settings";
        public const string PropOnUseEnabled = "on_use_enabled";
        public const string PropOnUse = "on_use_code";
        public const string PropOnStartEnabled = "on_start_enabled";
        public const string PropOnStart = "on_start_code";
        public const string PropOnStartSource = "on_start_code_source";
        public const string PropOnTimerEnabled = "on_timer_enabled";
        public const string PropOnTimer = "on_timer_code";
        public const string PropOnTimerRate = "on_timer_rate";
        public const string PropOnStopEnabled = "on_stop_enabled";
        public const string PropOnStop = "on_stop_code";
        public const string PropComboValues = "values";
        public const string PropValueType = "value_type";
        public const string PropInputWidth = "input_width";
        public const string PropStep = "step";
        public const string PropUsePanelDir = "use_panel_dir";
        public const string PropLogFileDir = "log_file_dir";
        public const string PropLogFile = "log_file";
        public const string PropLogFileFormat = "log_file_format";
        public const string PropUseSuffix = "use_suffix";
        public const string PropLoggingOnStart = "start_logging_on_start";
        public const string PropUseSlowerUpdateRate = "use_slower_update_rate";
        public const string PropSlowerUpdateRate = "slower_update_rate";
        public const string PropConnectionIdentifier = "connection_identifier";
        public const string PropSerialPortSettings = "serial_port_settings";
        public const string PropSerialPortName = "serial_port_name";
        public const string PropGroupNamespace = "group_namespace";
        public const string PropCollapsed = "collapsed";
        public const string PropUseImage = "use_image";
        public const string PropImage = "image";
        public const string PropImageScaling = "image_scaling";
        public const string PropText = "text";
        public const string PropSubPanelMode = "sub_panel_mode";
        public const string PropModelCompTypes = "model_components_types";
        public const string PropModelComp = "model_component";
        public const string PropWidgetValue = "widget_value";
        public const string PropCsState = "state";
        public const string PropCsCaptureTimeInterval = "time_interval";
        public const string PropCsCaptureSampleRate = "sample_rate";
        public const string PropCsScopeTimeBase = "time_base";
        public const string PropCsCaptureBg = "capture_background";
        public const string PropCsScopeBg = "scope_background";
        public const string PropCsCaptureLegend = "capture_legend";
        public const string PropCsScopeLegend = "scope_legend";
        public const string PropCsCaptureLayout = "capture_layout";
        public const string PropCsScopeLayout = "scope_layout";
        public const string PropCsCaptureSignals = "capture_signals";
        public const string PropCsScopeSignals = "scope_signals";
        public const string PropCsScopeTrigger = "scope_trigger";
        public const string PropCsCaptureTrigger = "capture_trigger";
        public const string PropCsActiveCapturePreset = "active_capture_preset";
        public const string PropCsActiveScopePreset = "active_scope_preset";

        // Widget action constants
        public const string ActCsForceTrigger = "force_trigger";
        public const string ActCsEnableTrigger = "enable_trigger";
        public const string ActCsStopCapture = "stop_capture";
        public const string ActCsExportData = "export_data";

        // Widget type constants
        public const string WtMacro = "Macro";
        public const string WtButtonMacro = "MacroButton";
        public const string WtTextMacro = "TextBoxMacro";
        public const string WtComboMacro = "ComboBoxMacro";
        public const string WtCheckboxMacro = "CheckBoxMacro";
        public const string WtSliderMacro = "SliderMacro";
        public const string WtKnobMacro = "KnobMacro";
        public const string WtGauge = "Gauge";
        public const string WtDigital = "DigitalDisplay";
        public const string WtText = "TextDisplay";
        public const string WtLed = "LedDisplay";
        public const string WtTrace = "TraceDisplay";
        public const string WtPv = "PVDisplay";
        public const string WtXyGraph = "XYGraphDisplay";
        public const string WtPhasorGraph = "PhasorGraphDisplay";
        public const string WtBarGraph = "BarGraphDisplay";
        public const string WtGroup = "Group";
        public const string WtSubPanel = "SubPanel";
        public const string WtLibraryCategory = "LibraryCategory";
        public const string WtTextNote = "TextNote";
        public const string WtImage = "Image";
        public const string WtSerialComm = "SerialComm";
        public const string WtCaptureScope = "Capture/Scope";
        public const string WtSignalDataLogger = "SignalDataLogger";
        public const string WtStreamingDataLogger = "StreamingSignalDataLogger";
        public const string WtFrequencyResponse = "FrequencyResponse";

        // Capture/Scope signal type constants
        public const string CsSigTA = "Analog";
        public const string CsSigTD = "Digital";
        public const string CsSigTV = "Virtual";
    }


    public class ScadaAPI : AbsractAPI
    {
        public override int ProperPort => Ports.ScadaApiPort;

        protected override JObject HandleRequest(string method, JObject parameters)
        {
            var res = Request(method, parameters);
            if (!res.TryGetValue("error", out var error)) return res;
            var msg = (string)error["message"]!;
            throw new ScadaAPIException(msg);
        }
    }
}
