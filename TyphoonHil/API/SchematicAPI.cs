using Newtonsoft.Json.Linq;
using TyphoonHil.Communication;
using TyphoonHil.Exceptions;

namespace TyphoonHil.API;

public class Position
{
    public Position(double x, double y)
    {
        X = x;
        Y = y;
    }

    public double X { get; set; }
    public double Y { get; set; }
    public JArray JArray => new() { X, Y };

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}

public class Size
{
    public Size(double width, double height)
    {
        Width = width;
        Height = height;
    }

    public Size(JArray jArray)
    {
        Width = (double)jArray[0];
        Height = (double)jArray[1];
    }

    public double Width { get; set; }
    public double Height { get; set; }
    public JArray JArray => new() { Width, Height }; //check if they should swap order

    public override bool Equals(object? obj)
    {
        if (obj is not Size objAsSize) return false;

        return Math.Abs(objAsSize.Height - Height) < 0.01 && Math.Abs(objAsSize.Width - Width) < 0.01;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height);
    }
}

public class Dimension
{
    public Dimension(double width, double height)
    {
        Width = width;
        Height = height;
    }

    public Dimension(double width)
    {
        Width = width;
    }

    public Dimension(JArray jArray)
    {
        Width = (double)jArray[0];
        Height = (double?)jArray[1];
    }

    public double Width { get; set; }
    public double? Height { get; set; }
    public JArray JArray => new() { Width, Height }; //check if they should swap order

    public override string ToString()
    {
        return $"({Width}, {Height})";
    }
}

public static class Rotation
{
    public const string Down = "down";
    public const string Up = "up";
    public const string Right = "right";
    public const string Left = "left";
}

public static class Flip
{
    public const string None = "none";
    public const string Horizontal = "horizontal";
    public const string Vertical = "vertical";
    public const string Both = "both";
}

public class Kind
{
    public const string Pe = "pe";
    public const string Sp = "sp";
}

public class TerminalPosition
{
    public const string Bottom = "bottom";
    public const string Top = "top";
    public const string Auto = "auto";

    public TerminalPosition(string first, string second)
    {
        First = first;
        Second = second;
    }

    public string First { get; set; }
    public string Second { get; set; }
    public JArray JArray => new() { First, Second };
}

public class Direction
{
    public const string In = "in";
    public const string Out = "out";
}

public class SpType
{
    public const string Inherit = "inherit";
    public const string Int = "int";
    public const string Uint = "uint";
    public const string Real = "real";
}

public class ItemType
{
    public const string Any = "unknown";
    public const string Component = "component";
    public const string MaskedComponent = "masked_component";
    public const string Mask = "mask";
    public const string Connection = "connection";
    public const string Tag = "tag";
    public const string Port = "port";
    public const string Comment = "comment";
    public const string Junction = "junction";
    public const string Terminal = "terminal";
    public const string Property = "property";
    public const string Signal = "signal";
    public const string SignalRef = "signal_ref";
}

public class RecursionStrategy
{
    public const string None = "none";
    public const string RecurseIntoLinkedComps = "recurse_linked_components";
}

public class ErrorType
{
    public const string General = "General error";
    public const string PropertyValueInvalid = "Invalid property value";
}

public class WarningType
{
    public const string General = "General warning";
}

public class HandlerName
{
    public const string ModelInit = "model_init";
    public const string ModelLoaded = "model_loaded";
    public const string Open = "open";
    public const string Init = "init";
    public const string MaskInit = "mask_init";
    public const string ConfigurationChanged = "configuration_changed";
    public const string PreCompile = "pre_compile";
    public const string BeforeChange = "before_change";
    public const string PreValidate = "pre_validate";
    public const string OnDialogOpen = "on_dialog_open";
    public const string OnDialogClose = "on_dialog_close";
    public const string CalcType = "calc_type";
    public const string CalcDimension = "calc_dimension";
    public const string ButtonClicked = "button_clicked";
    public const string DefineIcon = "define_icon";
    public const string PostResolve = "post_resolve";
    public const string PreCopy = "pre_copy";
    public const string PostCopy = "post_copy";
    public const string PreDelete = "pre_delete";
    public const string PostDelete = "post_delete";
    public const string NameChanged = "name_changed";
    public const string PostCCodeExport = "post_c_code_export";
    public const string MaskPreCompile = "mask_pre_cmpl";
    public const string PropertyValueChanged = "property_value_changed";
    public const string PropertyValueEdited = "property_value_edited";
}

public class Widget
{
    public const string Combo = "combo";
    public const string Edit = "edit";
    public const string Checkbox = "checkbox";
    public const string Button = "button";
    public const string ToggleButton = "togglebutton";
    public const string SignalChooser = "signal_chooser";
    public const string SignalAccess = "signal_access";
}

public class IconRotate
{
    public const string Rotate = "rotate";
    public const string NoRotate = "no_rotate";
    public const string TextLike = "text_like";
}

public class SchematicAPI : AbstractAPI
{
    private static readonly string FqnSep = ".";

    public SchematicAPI()
    {
    }

    internal SchematicAPI(ICommunication communication) : base(communication)
    {
    }

    protected override int ProperPort => Ports.SchematicApiPort;

    public JObject Load(string filename)
    {
        return Request("load", new JObject { { "filename", filename } });
    }

    public void Save()
    {
        HandleRequest("save", new JObject());
    }

    public void SaveAs(string filename)
    {
        HandleRequest("save_as", new JObject { { "filename", filename } });
    }

    public bool Compile()
    {
        return Request("compile", new JObject()).ContainsKey("result");
    }

    public void CreateNewModel(string? name = null)
    {
        Request("create_new_model", new JObject { { "name", name } });
    }

    public void CloseModel()
    {
        Request("close_model", new JObject());
    }

    public JObject CreateComponent(string typeName,
        JObject? parent = null,
        string? name = null,
        string rotation = Rotation.Up,
        string flip = Flip.None,
        Position? position = null,
        Size? size = null,
        bool hideName = false)
    {
        var parameters = new JObject
        {
            { "type_name", typeName },
            { "parent", parent },
            { "name", name },
            { "rotation", rotation },
            { "flip", flip },
            { "hide_name", hideName },
            { "position", position?.JArray },
            { "size", size?.JArray }
        };

        var res = HandleRequest("create_component", parameters);
        return (JObject)res["result"]!;
    }

    public JObject CreateJunction(string? name = null, JObject? parent = null, string kind = Kind.Pe,
        Position? position = null)
    {
        var parameters = new JObject
        {
            { "name", name },
            { "parent", parent },
            { "kind", kind },
            { "position", position?.JArray }
        };

        return (JObject)Request("create_junction", parameters)["result"]!;
    }


    public JObject CreatePort(string? name = null,
        JObject? parent = null,
        string? label = null,
        string kind = Kind.Pe,
        string direction = Direction.Out,
        Dimension? dimension = null,
        string spType = SpType.Real,
        TerminalPosition? terminalPosition = null,
        string rotation = Rotation.Up,
        string flip = Flip.None,
        bool hideName = false,
        Position? position = null)
    {
        dimension ??= new Dimension(1);
        var parameters = new JObject
        {
            { "name", name },
            { "parent", parent },
            { "label", label },
            { "kind", kind },
            { "direction", direction },
            { "dimension", dimension.JArray },
            { "sp_type", spType },
            { "terminal_position", terminalPosition?.JArray },
            { "rotation", rotation },
            { "flip", flip },
            { "hide_name", hideName },
            { "position", position?.JArray }
        };

        return (JObject)Request("create_port", parameters)["result"]!;
    }

    public JObject Term(JObject componentHandle, string terminalName)
    {
        var parameters = new JObject
        {
            { "term_name", terminalName },
            { "comp_handle", componentHandle }
        };
        return (JObject)Request("term", parameters)["result"]!;
    }

    public JObject
        CreateConnection(JObject start, JObject end, string? name = null,
            List<Position>? breakpoints = null) //check what breakpoints are
    {
        var parameters = new JObject
        {
            { "name", name },
            { "start", start },
            { "end", end },
            { "breakpoints", breakpoints is null ? null : new JArray { breakpoints.Select(bp => bp.JArray) } }
        };

        return (JObject)HandleRequest("create_connection", parameters)["result"]!;
    }

    public JObject SetPropertyValue(JObject propertyHandle, object value)
    {
        var parameters = new JObject
        {
            { "prop_handle", propertyHandle },
            { "value", JToken.FromObject(value) }
        };

        return Request("set_property_value", parameters);
    }

    public JObject Prop(JObject itemHandle, string propertyName)
    {
        var parameters = new JObject
        {
            { "prop_name", propertyName },
            { "item_handle", itemHandle }
        };

        return (JObject)HandleRequest("prop", parameters)["result"]!;
    }

    public void ReloadLibraries()
    {
        Request("reload_libraries", new JObject());
    }

    public List<string> GetLibraryPaths()
    {
        return ((JArray)Request("get_library_paths", new JObject())["result"]!).Select(item => item.ToString())
            .ToList();
    }

    public void AddLibraryPath(string libraryPath, bool addSubdirs = false, bool persist = false)
    {
        var parameters = new JObject
        {
            { "library_path", libraryPath },
            { "add_subdirs", addSubdirs },
            { "persist", persist }
        };

        Request("add_library_path", parameters);
    }

    public void RemoveLibraryPath(string libraryPath, bool persist = false)
    {
        var parameters = new JObject
        {
            { "library_path", libraryPath },
            { "persist", persist }
        };

        Request("remove_library_path", parameters);
    }

    public JObject CreateComment(string text, JObject? parent = null, string? name = null, Position? position = null)
    {
        var parameters = new JObject
        {
            { "text", text },
            { "parent", parent },
            { "name", name },
            { "position", position?.JArray }
        };

        var res = HandleRequest("create_comment", parameters);
        return (JObject)res["result"]!;
    }

    public void CreateLibraryModel(string libraryName, string fileName)
    {
        var parameters = new JObject
        {
            { "lib_name", libraryName },
            { "file_name", fileName }
        };

        HandleRequest("create_library_model", parameters);
    }

    public JObject CreateMask(JObject itemHandle)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle }
        };

        var res = HandleRequest("create_mask", parameters);
        return (JObject)res["result"]!;
    }

    public void DeleteItem(JObject itemHandle)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle }
        };

        HandleRequest("delete_item", parameters);
    }

    public JObject? DetectHwSettings() // Consider creating new Model with hw configuration
    {
        try
        {
            var res = HandleRequest("detect_hw_settings", new JObject());
            return (JObject)res["result"]!;
        }
        catch (SchematicAPIException)
        {
            return null;
        }
    }

    public List<JObject> DisableItems(List<JObject?> itemHandles)
    {
        var parameters = new JObject
        {
            { "item_handles", new JArray { itemHandles } }
        };

        var res = HandleRequest("disable_items", parameters);
        return ((JArray)res["result"]!).Select(item => (JObject)item).ToList();
    }

    public List<JObject> EnableItems(List<JObject?> itemHandles)
    {
        var parameters = new JObject
        {
            { "item_handles", new JArray { itemHandles } }
        };

        var res = HandleRequest("enable_items", parameters);
        return ((JArray)res["result"]!).Select(item => (JObject)item).ToList();
    }

    public bool IsEnabled(JObject itemHandle)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle }
        };

        return (bool)HandleRequest("is_enabled", parameters)["result"]!;
    }

    public JObject? GetItem(string name, JObject? parent = null, string itemType = ItemType.Any)
    {
        var parameters = new JObject
        {
            { "name", name },
            { "item_type", itemType },
            { "parent", parent }
        };

        return HandleRequest("get_item", parameters)["result"]?.Value<JObject>();
    }

    public void DisableProperty(JObject itemHandle)
    {
        var parameters = new JObject
        {
            { "prop_handle", itemHandle }
        };

        HandleRequest("disable_property", parameters);
    }

    public void DisablePropertySerialization(JObject itemHandle)
    {
        var parameters = new JObject
        {
            { "prop_handle", itemHandle }
        };

        HandleRequest("disable_property_serialization", parameters);
    }

    public void EnableProperty(JObject propHandle)
    {
        var parameters = new JObject
        {
            { "prop_handle", propHandle }
        };

        HandleRequest("enable_property", parameters);
    }

    public void EnablePropertySerialization(JObject itemHandle)
    {
        var parameters = new JObject
        {
            { "prop_handle", itemHandle }
        };

        HandleRequest("enable_property_serialization", parameters);
    }

    public bool IsPropertyEnabled(JObject propHandle)
    {
        var parameters = new JObject
        {
            { "prop_handle", propHandle }
        };

        return (bool)HandleRequest("is_property_enabled", parameters)["result"]!;
    }

    public bool IsPropertySerializable(JObject propHandle)
    {
        var parameters = new JObject
        {
            { "prop_handle", propHandle }
        };

        return (bool)HandleRequest("is_property_serializable", parameters)["result"]!;
    }

    public void Error(string msg, string kind = ErrorType.General, JObject? context = null)
    {
        var parameters = new JObject
        {
            { "msg", msg },
            { "kind", kind },
            { "context", context }
        };

        HandleRequest("error", parameters);
    }

    public bool Exists(string name, JObject? parent = null, string itemType = ItemType.Any)
    {
        var parameters = new JObject
        {
            { "name", name },
            { "parent", parent },
            { "item_type", itemType }
        };

        return (bool)HandleRequest("exists", parameters)["result"]!;
    }

    public void ExportModelToJson(string? outputDir = null, string recursionStrategy = RecursionStrategy.None)
    {
        var parameters = new JObject
        {
            { "output_dir", outputDir },
            { "recursion_strategy", recursionStrategy }
        };

        HandleRequest("export_model_to_json", parameters);
    }

    public List<JObject> FindConnections(JObject connectableHandle1, JObject? connectableHandle2 = null)
    {
        var parameters = new JObject
        {
            { "connectable_handle1", connectableHandle1 },
            { "connectable_handle2", connectableHandle2 }
        };

        return ((JArray)HandleRequest("find_connections", parameters)["result"]!).Select(item => (JObject)item)
            .ToList();
    }

    public static string Fqn(params string[] args)
    {
        var parts = args.SelectMany(arg => arg.Split(new[] { FqnSep }, StringSplitOptions.None)).Skip(1) // Ask why
            .Where(p => !string.IsNullOrWhiteSpace(p));
        return string.Join(FqnSep, parts);
    }

    public List<string> GetAvailableLibraryComponents(string libraryName = "")
    {
        var parameters = new JObject
        {
            { "library_name", libraryName }
        };

        return ((JArray)HandleRequest("get_available_library_components", parameters)["result"]!)
            .Select(item => (string)item!).ToList();
    }

    public List<Position> GetBreakpoints(JObject itemHandle)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle }
        };

        return ((JArray)HandleRequest("get_breakpoints", parameters)["result"]!)
            .Select(coordinates => new Position((double)coordinates[0]!, (double)coordinates[1]!)).ToList();
    }

    public string GetCommentText(JObject commentHandle)
    {
        var parameters = new JObject
        {
            { "comment_handle", commentHandle }
        };

        return (string)HandleRequest("get_comment_text", parameters)["result"]!;
    }

    public string GetCompiledModelFile(string schPath)
    {
        var parameters = new JObject
        {
            { "sch_path", schPath }
        };

        return (string)HandleRequest("get_compiled_model_file", parameters)["result"]!;
    }

    public JObject GetComponentTypeName(JObject compHandle)
    {
        var parameters = new JObject
        {
            { "comp_handle", compHandle }
        };

        return (JObject)HandleRequest("get_component_type_name", parameters)["result"]!;
    }

    public string GetConnectableDirection(JObject connectableHandle)
    {
        var parameters = new JObject
        {
            { "connectable_handle", connectableHandle }
        };

        return (string)HandleRequest("get_connectable_direction", parameters)["result"]!;
    }

    public string GetConnectableKind(JObject connectableHandle)
    {
        var parameters = new JObject
        {
            { "connectable_handle", connectableHandle }
        };

        return (string)HandleRequest("get_connectable_kind", parameters)["result"]!;
    }

    public List<JObject> GetConnectedItems(JObject itemHandle)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle }
        };

        return ((JArray)HandleRequest("get_connected_items", parameters)["result"]!).ToObject<List<JObject>>()!;
    }

    public string GetName(JObject itemHandle)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle }
        };

        return (string)HandleRequest("get_name", parameters)["result"]!;
    }

    public string GetConvProp(JObject propHandle, string? value = null) // ask more about solution
    {
        var parameters = new JObject
        {
            { "prop_handle", propHandle },
            { "value", value }
        };

        return (string)HandleRequest("get_conv_prop", parameters)["result"]!;
    }

    public string GetDescription(JObject itemHandle)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle }
        };

        return (string)HandleRequest("get_description", parameters)["result"]!;
    }

    public string GetFqn(JObject itemHandle)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle }
        };

        return (string)HandleRequest("get_fqn", parameters)["result"]!;
    }

    public string GetHandlerCode(JObject itemHandle, string handlerName)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle },
            { "handler_name", handlerName }
        };

        return (string)HandleRequest("get_handler_code", parameters)["result"]!;
    }

    public void SetHandlerCode(JObject itemHandle, string handlerName, string code)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle },
            { "handler_name", handlerName },
            { "code", code }
        };

        HandleRequest("set_handler_code", parameters);
    }

    public JObject CreateProperty(JObject itemHandle, string name, string label = "", string widget = "edit",
        JArray? comboValues = null, bool evaluate = true, bool enabled = true, bool visible = true,
        bool serializable = true, string tabName = "", string unit = "", string buttonLabel = "",
        JArray? previousNames = null, string description = "", string require = "", string type = "",
        string? defaultValue = null, string? minValue = null, string? maxValue = null,
        bool keepline = false, string? skip = null, string? skipStep = null,
        bool vector = false, bool tunable = false, string? index = null)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle },
            { "name", name },
            { "label", label },
            { "widget", widget },
            { "combo_values", comboValues },
            { "evaluate", evaluate },
            { "enabled", enabled },
            { "visible", visible },
            { "serializable", serializable },
            { "tab_name", tabName },
            { "unit", unit },
            { "button_label", buttonLabel },
            { "previous_names", previousNames },
            { "description", description },
            { "require", require },
            { "type", type },
            { "default_value", defaultValue },
            { "min_value", minValue },
            { "max_value", maxValue },
            { "keepline", keepline },
            { "skip", skip },
            { "skip_step", skipStep },
            { "vector", vector },
            { "tunable", tunable },
            { "index", index }
        };

        return (JObject)HandleRequest("create_property", parameters)["result"]!;
    }

    public string GetHwProperty(string propName)
    {
        var parameters = new JObject
        {
            { "prop_name", propName }
        };

        return (string)HandleRequest("get_hw_property", parameters)["result"]!;
    }

    public JObject? GetHwSettings()
    {
        try
        {
            var res = HandleRequest("get_hw_settings", new JObject());
            return (JObject)res["result"]!;
        }
        catch (SchematicAPIException)
        {
            return null;
        }
    }

    public string GetIconDrawingCommands(JObject itemHandle)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle }
        };

        return (string)HandleRequest("get_icon_drawing_commands", parameters)["result"]!;
    }

    public void SetIconDrawingCommands(JObject itemHandle, string drawingCommands)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle },
            { "drawing_commands", drawingCommands }
        };

        HandleRequest("set_icon_drawing_commands", parameters);
    }

    public List<JObject> GetItems(JObject? parent = null, string itemType = ItemType.Any)
    {
        var parameters = new JObject
        {
            { "parent", parent },
            { "item_type", itemType }
        };

        return ((JArray)HandleRequest("get_items", parameters)["result"]!).ToObject<List<JObject>>()!;
    }

    public JObject CreateTag(string value, string? name = null, JObject? parent = null,
        string scope = "global", string kind = Kind.Pe, string? direction = null,
        string rotation = Rotation.Up, string flip = Flip.None, Position? position = null)
    {
        var parameters = new JObject
        {
            { "value", value },
            { "name", name },
            { "parent", parent },
            { "scope", scope },
            { "kind", kind },
            { "direction", direction },
            { "rotation", rotation },
            { "flip", flip },
            { "position", position?.JArray }
        };

        return (JObject)HandleRequest("create_tag", parameters)["result"]!;
    }

    public string GetLabel(JObject itemHandle)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle }
        };

        return (string)HandleRequest("get_label", parameters)["result"]!;
    }

    public string GetLibraryResourceDirPath(JObject itemHandle)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle }
        };

        return (string)HandleRequest("get_library_resource_dir_path", parameters)["result"]!;
    }

    public JObject GetMask(JObject itemHandle)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle }
        };

        return (JObject)HandleRequest("get_mask", parameters)["result"]!;
    }

    public List<string> GetModelDependencies()
    {
        var parameters = new JObject();

        return ((JArray)HandleRequest("get_model_dependencies", parameters)["result"]!).ToObject<List<string>>()!;
    }

    public string GetModelFilePath()
    {
        return (string)HandleRequest("get_model_file_path", new JObject())["result"]!;
    }

    public JObject GetModelInformation()
    {
        return (JObject)HandleRequest("get_model_information", new JObject())["result"]!;
    }

    public string GetModelPropertyValue(string propCodeName) // consider another solution for dynamic types
    {
        var parameters = new JObject
        {
            { "prop_code_name", propCodeName }
        };

        return (string)HandleRequest("get_model_property_value", parameters)["result"]!;
    }

    public string GetNamespaceVariable(string varName)
    {
        var parameters = new JObject
        {
            { "var_name", varName }
        };

        return (string)HandleRequest("get_ns_var", parameters)["result"]!;
    }

    public JObject GetNamespaceVariables()
    {
        return (JObject)HandleRequest("get_ns_vars", new JObject())["result"]!;
    }

    public void SetNamespaceVariable(string varName, object value)
    {
        var parameters = new JObject
        {
            { "var_name", varName },
            { "value", JToken.FromObject(value) }
        };

        HandleRequest("set_ns_var", parameters);
    }

    public JObject GetParent(JObject itemHandle)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle }
        };

        return (JObject)HandleRequest("get_parent", parameters)["result"]!;
    }

    public Position GetPosition(JObject itemHandle)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle }
        };
        var res = (JArray)HandleRequest("get_position", parameters)["result"]!;
        return new Position((double)res[0], (double)res[1]);
    }

    public void SetPosition(JObject itemHandle, Position position)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle },
            { "position", position.JArray }
        };

        HandleRequest("set_position", parameters);
    }

    public List<string> GetPropertyComboValues(JObject propHandle)
    {
        var parameters = new JObject
        {
            { "prop_handle", propHandle }
        };

        return ((JArray)HandleRequest("get_property_combo_values", parameters)["result"]!).ToObject<List<string>>()!;
    }

    public void SetPropertyComboValues(JObject propHandle, List<string> comboValues)
    {
        var parameters = new JObject
        {
            { "prop_handle", propHandle },
            { "combo_values", new JArray(comboValues) }
        };

        HandleRequest("set_property_combo_values", parameters);
    }

    public string GetPropertyDefaultValue(JObject propHandle)
    {
        var parameters = new JObject
        {
            { "prop_handle", propHandle }
        };

        return (string)HandleRequest("get_property_default_value", parameters)["result"]!;
    }

    public string GetPropertyDisplayValue(JObject propHandle)
    {
        var parameters = new JObject
        {
            { "prop_handle", propHandle }
        };

        return (string)HandleRequest("get_property_disp_value", parameters)["result"]!;
    }

    public void SetPropertyDisplayValue(JObject propHandle, string value)
    {
        var parameters = new JObject
        {
            { "prop_handle", propHandle },
            { "value", value }
        };

        HandleRequest("set_property_disp_value", parameters);
    }

    public JObject GetPropertyTypeAttributes(JObject propHandle)
    {
        var parameters = new JObject
        {
            { "prop_handle", propHandle }
        };

        return (JObject)HandleRequest("get_property_type_attributes", parameters)["result"]!;
    }

    public string GetPropertyValue(JObject propHandle)
    {
        var parameters = new JObject
        {
            { "prop_handle", propHandle }
        };

        return (string)HandleRequest("get_property_value", parameters)["result"]!;
    }

    public string GetPropertyValueType(JObject propHandle)
    {
        var parameters = new JObject
        {
            { "prop_handle", propHandle }
        };

        return (string)HandleRequest("get_property_value_type", parameters)["result"]!;
    }

    public JObject GetPropertyValues(JObject itemHandle)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle }
        };

        return (JObject)HandleRequest("get_property_values", parameters)["result"]!;
    }

    public Size GetSize(JObject itemHandle)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle }
        };

        return new Size((JArray)HandleRequest("get_size", parameters)["result"]!);
    }

    public void SetSize(JObject itemHandle, int? width = null, int? height = null)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle },
            { "width", width },
            { "height", height }
        };

        HandleRequest("set_size", parameters);
    }

    public JObject GetSubLevelHandle(JObject itemHandle)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle }
        };

        return (JObject)HandleRequest("get_sub_level_handle", parameters)["result"]!;
    }

    public Dimension
        GetTerminalDimension(JObject terminalHandle) // Disccuss aabout return value, because it can return 'calc'
    {
        var parameters = new JObject
        {
            { "terminal_handle", terminalHandle }
        };

        return new Dimension((JArray)HandleRequest("get_terminal_dimension", parameters)["result"]!);
    }

    public void SetTerminalDimension(JObject terminalHandle, Dimension dimension)
    {
        var parameters = new JObject
        {
            { "terminal_handle", terminalHandle },
            { "dimension", dimension.JArray }
        };

        HandleRequest("set_terminal_dimension", parameters);
    }

    public string GetTerminalSpType(JObject terminalHandle)
    {
        var parameters = new JObject
        {
            { "terminal_handle", terminalHandle }
        };

        return (string)HandleRequest("get_terminal_sp_type", parameters)["result"]!;
    }

    public string GetTerminalSpTypeValue(JObject terminalHandle)
    {
        var parameters = new JObject
        {
            { "terminal_handle", terminalHandle }
        };

        return (string)HandleRequest("get_terminal_sp_type_value", parameters)["result"]!;
    }

    public void HideName(JObject itemHandle)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle }
        };

        HandleRequest("hide_name", parameters);
    }

    public void HideProperty(JObject propHandle)
    {
        var parameters = new JObject
        {
            { "prop_handle", propHandle }
        };

        HandleRequest("hide_property", parameters);
    }

    public void Info(string msg, JObject? context = null)
    {
        var parameters = new JObject
        {
            { "msg", msg },
            { "context", context }
        };

        HandleRequest("info", parameters);
    }

    public bool IsNameVisible(JObject itemHandle)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle }
        };

        return (bool)HandleRequest("is_name_visible", parameters)["result"]!;
    }

    public bool IsPropertyVisible(JObject propHandle)
    {
        var parameters = new JObject
        {
            { "prop_handle", propHandle }
        };

        return (bool)HandleRequest("is_property_visible", parameters)["result"]!;
    }

    public bool IsRequireSatisfied(string requireString)
    {
        var parameters = new JObject
        {
            { "require_string", requireString }
        };

        return (bool)HandleRequest("is_require_satisfied", parameters)["result"]!;
    }

    public bool IsSubsystem(JObject compHandle)
    {
        var parameters = new JObject
        {
            { "comp_handle", compHandle }
        };

        return (bool)HandleRequest("is_subsystem", parameters)["result"]!;
    }

    public bool IsTerminalFeedthrough(JObject terminalHandle)
    {
        var parameters = new JObject
        {
            { "terminal_handle", terminalHandle }
        };

        return (bool)HandleRequest("is_terminal_feedthrough", parameters)["result"]!;
    }

    public bool IsTunable(JObject itemHandle)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle }
        };

        return (bool)HandleRequest("is_tunable", parameters)["result"]!;
    }

    public string ModelToApi() // ask about params // ask how to convert result
    {
        return (string)HandleRequest("model_to_api")["result"]!;
    }

    public void PrecompileFmu(string fmuFilePath, string? precompiledFilePath = null,
        List<string>? additionalDefinitions = null)
    {
        var parameters = new JObject
        {
            { "fmu_file_path", fmuFilePath },
            { "precompiled_file_path", precompiledFilePath },
            { "additional_definitions", new JArray { additionalDefinitions } }
        };
        HandleRequest("precompile_fmu", parameters);
    }

    public void PrintMessage(string message)
    {
        var parameters = new JObject
        {
            { "message", message }
        };
        HandleRequest("print_message", parameters);
    }

    public void RefreshIcon(JObject itemHandle)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle }
        };

        HandleRequest("refresh_icon", parameters);
    }

    public void SetComponentIconImage(JObject itemHandle, string imageFilename, string rotate = IconRotate.Rotate)
    {
        var parameters = new JObject
        {
            { "image_filename", imageFilename },
            { "item_handle", itemHandle },
            { "rotate", rotate }
        };
        HandleRequest("set_component_icon_image", parameters);
    }

    public void SetColor(JObject itemHandle, string color)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle },
            { "color", color }
        };

        HandleRequest("set_color", parameters);
    }

    public void RemoveMask(JObject itemHandle)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle }
        };
        HandleRequest("remove_mask", parameters);
    }

    public void RemoveProperty(JObject itemHandle, string name)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle },
            { "name", name }
        };

        HandleRequest("remove_property", parameters);
    }

    public bool SetComponentProperty(string component, string property, string value)
    {
        var parameters = new JObject
        {
            { "value", value },
            { "component", component },
            { "property", property }
        };

        return (bool)HandleRequest("set_component_property", parameters)["result"]!;
    }

    public void SetDescription(JObject itemHandle, string description)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle },
            { "description", description }
        };

        HandleRequest("set_description", parameters);
    }

    public bool SetHwSettings(string product, string revision, string confId)
    {
        var parameters = new JObject
        {
            { "product", product },
            { "revision", revision },
            { "conf_id", confId }
        };

        return (bool)HandleRequest("set_hw_settings", parameters)["result"]!;
    }

    public void SetLabel(JObject itemHandle, string label)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle },
            { "label", label }
        };

        HandleRequest("set_label", parameters);
    }

    public void SetModelDependencies(List<string> dependenciesList)
    {
        var parameters = new JObject
        {
            { "dependencies_list", new JArray(dependenciesList) }
        };

        HandleRequest("set_model_dependencies", parameters);
    }

    public void SetModelInitCode(string code)
    {
        var parameters = new JObject
        {
            { "code", code }
        };

        HandleRequest("set_model_init_code", parameters);
    }

    public void SetModelPropertyValue(string propCodeName, object value)
    {
        var parameters = new JObject
        {
            { "prop_code_name", propCodeName },
            { "value", JToken.FromObject(value) }
        };

        HandleRequest("set_model_property_value", parameters);
    }

    public void SetName(JObject itemHandle, string name)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle },
            { "name", name }
        };

        HandleRequest("set_name", parameters);
    }

    public void SetPortProperties(JObject itemHandle, string? terminalPosition = null, bool? hideTermLabel = null,
        string termLabel = "")
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle },
            { "terminal_position", terminalPosition },
            { "hide_term_label", hideTermLabel },
            { "term_label", termLabel }
        };
        HandleRequest("set_port_properties", parameters);
    }

    [Obsolete("This function is deprecated")]
    public bool SetPropertyAttribute(JObject component, JObject property, string attribute, string value)
    {
        var parameters = new JObject
        {
            { "component", component },
            { "property", property },
            { "attribute", attribute },
            { "value", value }
        };

        return (bool)HandleRequest("set_property_attribute", parameters)["result"]!;
    }

    public void SetPropertyValueType(JObject propHandle, string newType)
    {
        var parameters = new JObject
        {
            { "prop_handle", propHandle },
            { "new_type", newType }
        };

        HandleRequest("set_property_value_type", parameters);
    }

    public void SetPropertyValues(JObject itemHandle, JObject values)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle },
            { "values", values }
        };

        HandleRequest("set_property_values", parameters);
    }

    [Obsolete(
        "Deprecated since version 2.0: Use set_model_property_value instead (simulation_method field in configuration object).")]
    public void SetSimulationMethod(string simulationMethod)
    {
        var parameters = new JObject
        {
            { "simulation_method", simulationMethod }
        };

        HandleRequest("set_simulation_method", parameters);
    }

    public void SetSimulationTimeStep(double timeStep)
    {
        var parameters = new JObject
        {
            { "time_step", timeStep }
        };

        HandleRequest("set_simulation_time_step", parameters);
    }

    public void SetTagProperties(JObject itemHandle, string? value = null, string? scope = null)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle },
            { "value", value },
            { "scope", scope }
        };

        HandleRequest("set_tag_properties", parameters);
    }

    public void SetTerminalDimension(JObject terminalHandle, JObject dimension)
    {
        var parameters = new JObject
        {
            { "terminal_handle", terminalHandle },
            { "dimension", dimension }
        };

        HandleRequest("set_terminal_dimension", parameters);
    }

    public void SetTerminalFeedthrough(JObject terminalHandle, bool feedthrough)
    {
        var parameters = new JObject
        {
            { "terminal_handle", terminalHandle },
            { "feedthrough", feedthrough }
        };

        HandleRequest("set_terminal_feedthrough", parameters);
    }

    public void SetTerminalSpType(JObject terminalHandle, string spType)
    {
        var parameters = new JObject
        {
            { "terminal_handle", terminalHandle },
            { "sp_type", spType }
        };

        HandleRequest("set_terminal_sp_type", parameters);
    }

    public void SetTerminalSpTypeValue(JObject terminalHandle, string spTypeValue)
    {
        var parameters = new JObject
        {
            { "terminal_handle", terminalHandle },
            { "sp_type_value", spTypeValue }
        };

        HandleRequest("set_terminal_sp_type_value", parameters);
    }

    public void SetTunable(JObject itemHandle, bool value)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle },
            { "value", value }
        };

        HandleRequest("set_tunable", parameters);
    }

    public void ShowName(JObject itemHandle)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle }
        };

        HandleRequest("show_name", parameters);
    }

    public void ShowProperty(JObject propHandle)
    {
        var parameters = new JObject
        {
            { "prop_handle", propHandle }
        };

        HandleRequest("show_property", parameters);
    }

    public void SyncDynamicTerminals(JObject compHandle, string termName, int termNum, List<string>? labels = null,
        List<string>? spTypes = null, List<bool>? feedthroughs = null)
    {
        var parameters = new JObject
        {
            { "comp_handle", compHandle },
            { "term_name", termName },
            { "term_num", termNum },
            { "labels", labels != null ? new JArray(labels) : null },
            { "sp_types", spTypes != null ? new JArray(spTypes) : null },
            { "feedthroughs", feedthroughs != null ? new JArray(feedthroughs) : null }
        };

        HandleRequest("sync_dynamic_terminals", parameters);
    }

    public void UnlinkComponent(JObject itemHandle)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle }
        };

        HandleRequest("unlink_component", parameters);
    }

    public void Warning(string msg, string kind = WarningType.General, JObject? context = null)
    {
        var parameters = new JObject
        {
            { "msg", msg },
            { "kind", kind },
            { "context", context }
        };

        HandleRequest("warning", parameters);
    }

    public void ExportCFromSelection(List<JObject> compHandles, string outputDir)
    {
        var compHandleArray = new JArray(compHandles.Select(handle => handle));

        var parameters = new JObject
        {
            { "comp_handles", compHandleArray },
            { "output_dir", outputDir }
        };

        HandleRequest("export_c_from_selection", parameters);
    }

    public void ExportLibrary(string outputFile, List<string>? resourcePaths = null,
        List<string>? dependencyPaths = null, bool lockTopLevelComponents = true, bool encryptLibrary = true,
        bool encryptResourceFiles = true, List<string>? excludeFromEncryption = null)
    {
        var parameters = new JObject
        {
            { "output_file", outputFile },
            { "resource_paths", resourcePaths != null ? new JArray(resourcePaths) : null },
            { "dependency_paths", dependencyPaths != null ? new JArray(dependencyPaths) : null },
            { "lock_top_level_components", lockTopLevelComponents },
            { "encrypt_library", encryptLibrary },
            { "encrypt_resource_files", encryptResourceFiles },
            { "exclude_from_encryption", excludeFromEncryption != null ? new JArray(excludeFromEncryption) : null }
        };

        HandleRequest("export_library", parameters);
    }

    public void DisplayComponentIconText(JObject itemHandle, string text, string rotate = IconRotate.TextLike,
        int size = 10, double relposX = 0.5, double relposY = 0.5, double trimFactor = 1.0)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle },
            { "text", text },
            { "rotate", rotate },
            { "size", size },
            { "relpos_x", relposX },
            { "relpos_y", relposY },
            { "trim_factor", trimFactor }
        };

        HandleRequest("disp_component_icon_text", parameters);
    }

    public void ExportCFromSubsystem(JObject compHandle, string? outputDir = null)
    {
        var parameters = new JObject
        {
            { "comp_handle", compHandle },
            { "output_dir", outputDir }
        };

        HandleRequest("export_c_from_subsystem", parameters);
    }

    protected override JObject HandleRequest(string method, JObject parameters)
    {
        var res = Request(method, parameters);
        if (!res.ContainsKey("error")) return res;
        var msg = (string)res["error"]!["message"]!;
        throw new SchematicAPIException(msg);

    }
}