using Newtonsoft.Json.Linq;
using TyphoonHil.Exceptions;

namespace TyphoonHil.API;

public class ScadaConstants
{
    public const string Name = "name";
    public const string HtmlName = "html_name";
    public const string NamePosition = "name_position";
    public const string Label = "label";
    public const string UseLabel = "use_label";
    public const string Fqn = "fully_qualified_name";
    public const string Description = "description";
    public const string PanelInit = "panel_init_code";
    public const string PanelLock = "panel_locked";
    public const string Position = "position";
    public const string Size = "size";
    public const string Appearance = "appearance";
    public const string Signals = "signals";
    public const string StreamingSignals = "streaming_signals";
    public const string DataType = "data_type";
    public const string Expression = "expression_code";
    public const string UpdateRate = "update_rate";
    public const string TimeWindow = "time_window";
    public const string Unit = "unit";
    public const string AutoUnit = "auto_unit_assign";
    public const string BgColor = "bg_color";
    public const string PanelBgColor = "panel_bg_color";
    public const string BgType = "bg_type";
    public const string UseAsBg = "use_as_bg";
    public const string Range = "range";
    public const string UseColorRange = "use_color_range";
    public const string WarningRange = "warning_range";
    public const string CriticalRange = "critical_range";
    public const string GreenRange = "green_range";
    public const string OrangeRange = "orange_range";
    public const string Decimals = "decimals";
    public const string ScalingFactor = "scaling_factor";
    public const string StreamingAnSigScaling = "streaming_analog_signals_scaling";
    public const string AnSigScaling = "analog_signals_scaling";
    public const string RedRange = "red_range";
    public const string LedColor = "led_color";
    public const string XTitle = "x_title";
    public const string YTitle = "y_title";
    public const string XTitleEnabled = "x_title_enabled";
    public const string YTitleEnabled = "y_title_enabled";
    public const string CustomXTitle = "custom_x_title";
    public const string CustomYTitle = "custom_y_title";
    public const string CustomXTitleEnabled = "custom_x_title_enabled";
    public const string CustomYTitleEnabled = "custom_y_title_enabled";
    public const string XRange = "x_range";
    public const string YRange = "y_range";
    public const string AutoScaleEnabled = "autoscale_enabled";
    public const string XAutoScaleEnabled = "x_axis_autoscale_enabled";
    public const string YAutoScaleEnabled = "Y_axis_autoscale_enabled";
    public const string LegendEnabled = "legend_enabled";
    public const string RefCurveEnabled = "ref_curves_enabled";
    public const string RefCurve = "ref_curves_code";
    public const string PvPanel = "pv_panel";
    public const string LineStyle = "line_style";
    public const string PlotRange = "plot_range";
    public const string PhasorsSettings = "phasors_settings";
    public const string BarsSettings = "bars_settings";
    public const string OnUseEnabled = "on_use_enabled";
    public const string OnUse = "on_use_code";
    public const string OnStartEnabled = "on_start_enabled";
    public const string OnStart = "on_start_code";
    public const string OnStartSource = "on_start_code_source";
    public const string OnTimerEnabled = "on_timer_enabled";
    public const string OnTimer = "on_timer_code";
    public const string OnTimerRate = "on_timer_rate";
    public const string OnStopEnabled = "on_stop_enabled";
    public const string OnStop = "on_stop_code";
    public const string ComboValues = "values";
    public const string ValueType = "value_type";
    public const string InputWidth = "input_width";
    public const string Step = "step";
    public const string UsePanelDir = "use_panel_dir";
    public const string LogFileDir = "log_file_dir";
    public const string LogFile = "log_file";
    public const string LogFileFormat = "log_file_format";
    public const string UseSuffix = "use_suffix";
    public const string LoggingOnStart = "start_logging_on_start";
    public const string UseSlowerUpdateRate = "use_slower_update_rate";
    public const string SlowerUpdateRate = "slower_update_rate";
    public const string ConnectionIdentifier = "connection_identifier";
    public const string SerialPortSettings = "serial_port_settings";
    public const string SerialPortName = "serial_port_name";
    public const string GroupNamespace = "group_namespace";
    public const string Collapsed = "collapsed";
    public const string UseImage = "use_image";
    public const string Image = "image";
    public const string ImageScaling = "image_scaling";
    public const string Text = "text";
    public const string SubPanelMode = "sub_panel_mode";
    public const string ModelCompTypes = "model_components_types";
    public const string ModelComp = "model_component";
    public const string WidgetValue = "widget_value";
    public const string CsState = "state";
    public const string CsCaptureTimeInterval = "time_interval";
    public const string CsCaptureSampleRate = "sample_rate";
    public const string CsScopeTimeBase = "time_base";
    public const string CsCaptureBg = "capture_background";
    public const string CsScopeBg = "scope_background";
    public const string CsCaptureLegend = "capture_legend";
    public const string CsScopeLegend = "scope_legend";
    public const string CsCaptureLayout = "capture_layout";
    public const string CsScopeLayout = "scope_layout";
    public const string CsCaptureSignals = "capture_signals";
    public const string CsScopeSignals = "scope_signals";
    public const string CsScopeTrigger = "scope_trigger";
    public const string CsCaptureTrigger = "capture_trigger";
    public const string CsActiveCapturePreset = "active_capture_preset";
    public const string CsActiveScopePreset = "active_scope_preset";


    public const string ActCsForceTrigger = "force_trigger";
    public const string ActCsEnableTrigger = "enable_trigger";
    public const string ActCsStopCapture = "stop_capture";
    public const string ActCsExportData = "export_data";


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

    public const string CsSigTa = "Analog";
    public const string CsSigTd = "Digital";
    public const string CsSigTv = "Virtual";
}

public class ScadaAPI : AbstractAPI
{
    protected override int ProperPort => Ports.ScadaApiPort;

    protected override JObject HandleRequest(string method, JObject parameters)
    {
        var res = Request(method, parameters);
        if (!res.TryGetValue("error", out var error)) return res;
        var msg = (string)error["message"]!;
        throw new ScadaAPIException(msg);
    }

    public void AddLibraryPath(string libraryPath, bool addSubdirs = false, bool persist = false)
    {
        var parameters = new JObject
        {
            { "library_path", libraryPath },
            { "add_subdirs", addSubdirs },
            { "persist", persist }
        };

        HandleRequest("add_library_path", parameters);
    }


    public JObject CreateWidget(string widgetType, JObject? parent = null, string? name = null, Tuple<int,int>? position = null,
        string? linkToModelComp = null)
    {
        var parameters = new JObject
        {
            { "widget_type", widgetType },
            { "parent", parent },
            { "name", name },
            { "position", position is null ? null : new JArray{position.Item1, position.Item2}},
            { "link_to_model_comp", linkToModelComp }
        };

        return (JObject)HandleRequest("create_widget", parameters)["result"]!;
    }


    public void CreateNewPanel()
    {
        HandleRequest("create_new_panel", new JObject());
    }


    public void CreateNewLibraryPanel(string libraryName, string libraryDescription = "")
    {
        var parameters = new JObject
        {
            { "library_name", libraryName },
            { "library_description", libraryDescription }
        };

        HandleRequest("create_new_library_panel", parameters);
    }


    public JArray Copy(JObject srcHandle, JObject? dstHandle = null, string? name = null, Tuple<int, int>? position = null)
    {
        var parameters = new JObject
        {
            { "src_handle", srcHandle },
            { "dst_handle", dstHandle },
            { "name", name },
            { "position", position is null ? null : new JArray{ position.Item1, position.Item2 } }
        };

        return (JArray)HandleRequest("copy", parameters)["result"]!;
    }


    public void DeleteWidget(JObject widgetHandle)
    {
        var parameters = new JObject
        {
            { "widget_handle", widgetHandle }
        };

        HandleRequest("delete_widget", parameters);
    }


    public void ExecuteAction(string widgetHandle, string actionName, JObject parameters)
    {
        var requestParameters = new JObject
        {
            { "widget_handle", widgetHandle },
            { "action_name", actionName },
            { "params", parameters }
        };

        HandleRequest("execute_action", requestParameters);
    }


    public void ReloadLibraries()
    {
        HandleRequest("reload_libraries", new JObject());
    }


    public void RemoveLibraryPath(string libraryPath, bool persist = false)
    {
        var parameters = new JObject
        {
            { "library_path", libraryPath },
            { "persist", persist }
        };

        HandleRequest("remove_library_path", parameters);
    }


    public void LoadPanel(string panelFile)
    {
        var parameters = new JObject
        {
            { "panel_file", panelFile }
        };

        HandleRequest("load_panel", parameters);
    }


    public void LoadLibraryPanel(string libraryPanelFile)
    {
        var parameters = new JObject
        {
            { "library_panel_file", libraryPanelFile }
        };

        HandleRequest("load_library_panel", parameters);
    }


    public void SavePanel()
    {
        HandleRequest("save_panel", new JObject());
    }


    public void SavePanelAs(string saveTo)
    {
        var parameters = new JObject
        {
            { "save_to", saveTo }
        };

        HandleRequest("save_panel_as", parameters);
    }

    public void SetPropertyValue(JObject widgetHandle, string propName, object propValue)
    {
        var parameters = new JObject
        {
            { "widget_handle", widgetHandle },
            { "prop_name", propName },
            { "prop_value", JToken.FromObject(propValue) }
        };

        HandleRequest("set_property_value", parameters);
    }


    public string GetPropertyValue(JObject widgetHandle, string propName)
    {
        var parameters = new JObject
        {
            { "widget_handle", widgetHandle },
            { "prop_name", propName }
        };

        var result = HandleRequest("get_property_value", parameters);


        return (string)result["result"]!;
    }


    public JObject GetWidgetById(string widgetId)
    {
        var parameters = new JObject
        {
            { "widget_id", widgetId }
        };

        var result = HandleRequest("get_widget_by_id", parameters);


        return (JObject)result["result"]!;
    }


    public JObject GetWidgetByFqn(string widgetFqn, JObject? parent = null)
    {
        var parameters = new JObject
        {
            { "widget_fqn", widgetFqn },
            { "parent", parent }
        };

        var result = HandleRequest("get_widget_by_fqn", parameters);


        return (JObject)result["result"]!;
    }


    public List<string> GetLibraryPaths()
    {
        var result = HandleRequest("get_library_paths", new JObject());


        return ((JArray)result["result"]!).Select(item => item.ToString()).ToList();
    }
}