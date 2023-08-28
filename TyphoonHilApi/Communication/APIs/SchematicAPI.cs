using Newtonsoft.Json.Linq;
using System.Diagnostics;
using TyphoonHilApi.Communication.Exceptions;

namespace TyphoonHilApi.Communication.APIs
{
    internal class Position
    {
        public double X { get; set; }
        public double Y { get; set; }
        public JArray JArray => new() { X, Y };

        public Position(double x, double y)
        {
            X = x;
            Y = y;
        }
        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }

    internal class Size
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public JArray JArray => new() { Width, Height }; //check if they should swap order
        public Size(double width, double height)
        {
            Width = width;
            Height = height;
        }
    }

    internal class Dimension
    {
        public double Width { get; set; }
        public double? Height { get; set; }
        public JArray JArray => new() { Width, Height }; //check if they should swap order
        public Dimension(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public Dimension(double width)
        {
            Width = width;
        }
    }

    internal static class Rotation
    {
        public const string Down = "down";
        public const string Up = "up";
        public const string Right = "right";
        public const string Left = "left";
    }

    internal static class Flip
    {
        public const string None = "none";
        public const string Horizontal = "horizontal";
        public const string Vertical = "vertical";
        public const string Both = "both";
    }

    internal class Kind
    {
        public const string Pe = "pe";
        public const string Sp = "sp";
    }

    internal class TerminalPosition
    {
        public const string Bottom = "bottom";
        public const string Top = "top";
        public const string Auto = "auto";

        public string First { get; set; }
        public string Second { get; set; }
        public JArray JArray => new() { First, Second };
        public TerminalPosition(string first, string second)
        {
            First = first;
            Second = second;
        }
    }

    internal class Direction
    {
        public const string In = "in";
        public const string Out = "out";
    }

    internal class SPType
    {
        public const string Inherit = "inherit";
        public const string Int = "int";
        public const string Uint = "uint";
        public const string Real = "real";
    }

    internal class ItemType
    {
        public const string ANY = "unknown";
        public const string COMPONENT = "component";
        public const string MASKED_COMPONENT = "masked_component";
        public const string MASK = "mask";
        public const string CONNECTION = "connection";
        public const string TAG = "tag";
        public const string PORT = "port";
        public const string COMMENT = "comment";
        public const string JUNCTION = "junction";
        public const string TERMINAL = "terminal";
        public const string PROPERTY = "property";
        public const string SIGNAL = "signal";
        public const string SIGNAL_REF = "signal_ref";
    }

    internal class RecursionStrategy
    {
        public const string None = "none";
        public const string RECURSE_INTO_LINKED_COMPS = "recurse_linked_components";
    }

    internal class ErrorType
    {
        public const string General = "General error";
        public const string PROPERTY_VALUE_INVALID = "Invalid property value";
    }

    internal class HandlerName
    {
        public const string MODEL_INIT = "model_init";
        public const string MODEL_LOADED = "model_loaded";
        public const string OPEN = "open";
        public const string INIT = "init";
        public const string MASK_INIT = "mask_init";
        public const string CONFIGURATION_CHANGED = "configuration_changed";
        public const string PRE_COMPILE = "pre_compile";
        public const string BEFORE_CHANGE = "before_change";
        public const string PRE_VALIDATE = "pre_validate";
        public const string ON_DIALOG_OPEN = "on_dialog_open";
        public const string ON_DIALOG_CLOSE = "on_dialog_close";
        public const string CALC_TYPE = "calc_type";
        public const string CALC_DIMENSION = "calc_dimension";
        public const string BUTTON_CLICKED = "button_clicked";
        public const string DEFINE_ICON = "define_icon";
        public const string POST_RESOLVE = "post_resolve";
        public const string PRE_COPY = "pre_copy";
        public const string POST_COPY = "post_copy";
        public const string PRE_DELETE = "pre_delete";
        public const string POST_DELETE = "post_delete";
        public const string NAME_CHANGED = "name_changed";
        public const string POST_C_CODE_EXPORT = "post_c_code_export";
        public const string MASK_PRE_COMPILE = "mask_pre_cmpl";
        public const string PROPERTY_VALUE_CHANGED = "property_value_changed";
        public const string PROPERTY_VALUE_EDITED = "property_value_edited";
    }

    internal class Widget
    {
        public const string COMBO = "combo";
        public const string EDIT = "edit";
        public const string CHECKBOX = "checkbox";
        public const string BUTTON = "button";
        public const string TOGGLE_BUTTON = "togglebutton";
        public const string SIGNAL_CHOOSER = "signal_chooser";
        public const string SIGNAL_ACCESS = "signal_access";
    }

    internal class SchematicAPI : AbsractAPI
    {
        readonly static string FqnSep = ".";
        public SchematicAPI() { }
        public SchematicAPI(ICommunication communication) : base(communication) { }

        public override int ProperPort => Ports.SchematicApiPort;

        public JObject Load(string filename)
        {
            return Request("load", new JObject() { { "filename", filename } });
        }
        public JObject Save()
        {
            return Request("save", new());
        }

        public JObject SaveAs(string filename)
        {
            return Request("save_as", new JObject() { { "filename", filename } });
        }

        public bool Compile()
        {
            return Request("compile", new()).ContainsKey("result");
        }

        public JObject CreateNewModel()
        {
            return Request("create_new_model", new());
        }

        public JObject CloseModel()
        {
            return Request("close_model", new());
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
            JObject parameters = new JObject() {
                { "type_name", typeName },
                {"parent", parent },
                {"name", name },
                {"rotation", rotation},
                {"flip", flip},
                {"hide_name", hideName },
                {"position", position?.JArray },
                {"size", size?.JArray }
            };

            JObject res = HandleRequest("create_component", parameters);
            return (JObject)res["result"]!;
        }

        public JObject CreateJunction(string? name = null, JObject? parent = null, string kind = Kind.Pe, Position? position = null)
        {
            JObject parameters = new JObject()
            {
                { "name", name },
                {"parent", parent },
                {"kind", kind},
                {"position", position?.JArray },
            };

            return (JObject)Request("create_junction", parameters)["result"]!;
        }


        public JObject CreatePort(string? name = null,
                          JObject? parent = null,
                          string? label = null,
                          string kind = Kind.Pe,
                          string direction = Direction.Out,
                          Dimension? dimension = null,
                          string spType = SPType.Real,
                          TerminalPosition? terminalPosition = null,
                          string rotation = Rotation.Up,
                          string flip = Flip.None,
                          bool hideName = false,
                          Position? position = null)
        {
            dimension ??= new(1);
            JObject parameters = new JObject()
            {
                { "name", name },
                { "parent", parent },
                { "label", label },
                { "kind", kind },
                { "direction", direction },
                { "dimension", dimension?.JArray },
                { "sp_type", spType },
                { "terminal_position", terminalPosition?.JArray },
                { "rotation", rotation },
                { "flip", flip },
                { "hide_name", hideName },
                { "position", position?.JArray },
            };

            return (JObject)Request("create_port", parameters)["result"]!;
        }

        public JObject Term(JObject componentHandle, string terminalName)
        {
            JObject parameters = new JObject()
            {
                { "term_name", terminalName },
                { "comp_handle", componentHandle },
            };
            return (JObject)Request("term", parameters)["result"]!;
        }

        public JObject CreateConnection(JObject start, JObject end, string? name = null, List<Position>? breakpoints = null) //check what breakpoints are
        {
            JObject parameters = new JObject()
            {
                { "name", name },
                { "start", start },
                { "end", end },
                { "breakpoints", breakpoints is null ? null : new JArray() { breakpoints?.Select(bp => bp.JArray) } },
            };

            return (JObject)HandleRequest("create_connection", parameters)["result"]!;
        }

        public JObject SetPropertyValue(JObject propertyHandle, object value)
        {
            JObject parameters = new JObject()
            {
                { "prop_handle", propertyHandle },
                { "value", JToken.FromObject(value) },
            };

            return Request("set_property_value", parameters);
        }

        public JObject Prop(JObject itemHandle, string propertyName)
        {
            JObject parameters = new JObject()
            {
                { "prop_name", propertyName },
                { "item_handle", itemHandle },
            };

            return (JObject)HandleRequest("prop", parameters)["result"]!;
        }

        public JObject ReloadLibraries()
        {
            return Request("reload_libraries", new());
        }

        public List<string> GetLibraryPaths()
        {
            return ((JArray)Request("get_library_paths", new())["result"]!).Select(item => item.ToString()).ToList();
        }

        public JObject AddLibraryPath(string libraryPath, bool addSubdirs = false, bool persist = false)
        {
            JObject parameters = new JObject()
            {
                { "library_path", libraryPath },
                { "add_subdirs", addSubdirs },
                { "persist", persist },
            };

            return Request("add_library_path", parameters);
        }

        public JObject RemoveLibraryPath(string libraryPath, bool persist = false)
        {
            JObject parameters = new JObject()
            {
                { "library_path", libraryPath },
                { "persist", persist },
            };

            return Request("remove_library_path", parameters);
        }

        public JObject CreateComment(string text, JObject? parent = null, string? name = null, Position? position = null)
        {
            JObject parameters = new JObject()
            {
                { "text", text },
                { "parent", parent },
                { "name", name },
                { "position", position?.JArray }
            };

            JObject res = HandleRequest("create_comment", parameters);
            return (JObject)res["result"]!;
        }

        public void CreateLibraryModel(string libraryName, string fileName)
        {
            JObject parameters = new JObject()
            {
                { "lib_name", libraryName },
                { "file_name", fileName },
            };

            HandleRequest("create_library_model", parameters);
        }

        public JObject CreateMask(JObject itemHandle)
        {
            JObject parameters = new JObject()
            {
                { "item_handle", itemHandle },
            };

            JObject res = HandleRequest("create_mask", parameters);
            return (JObject)res["result"]!;
        }

        public void DeleteItem(JObject itemHandle)
        {
            JObject parameters = new JObject()
            {
                { "item_handle", itemHandle },
            };

            HandleRequest("delete_item", parameters);
        }

        public JObject? DetectHwSettings() // Consider creating new Model with hw configuration
        {
            try
            {
                JObject res = HandleRequest("detect_hw_settings", new());
                return (JObject)res["result"]!;
            }
            catch (SchematicAPIException)
            {
                return null;
            }
            throw new UnreachableException();
        }

        public List<JObject> DisableItems(List<JObject?> itemHandles)
        {
            JObject parameters = new JObject()
            {
                { "item_handles", new JArray() { itemHandles } }
            };

            JObject res = HandleRequest("disable_items", parameters);
            return ((JArray)res["result"]!).Select(item => (JObject)item).ToList();
        }

        public List<JObject> EnableItems(List<JObject?> itemHandles)
        {
            JObject parameters = new JObject()
            {
                { "item_handles", new JArray() { itemHandles } }
            };

            JObject res = HandleRequest("enable_items", parameters);
            return ((JArray)res["result"]!).Select(item => (JObject)item).ToList();
        }

        public bool IsEnabled(JObject itemHandle)
        {
            JObject parameters = new JObject()
            {
                { "item_handle", itemHandle }
            };

            return (bool)HandleRequest("is_enabled", parameters)["result"]!;
        }

        public JObject? GetItem(string name, JObject? parent = null, string itemType = ItemType.ANY)
        {
            JObject parameters = new JObject()
            {
                { "name", name },
                { "item_type", itemType },
                { "parent", parent },
            };

            return HandleRequest("get_item", parameters)["result"]?.Value<JObject>();
        }

        public void DisableProperty(JObject itemHandle)
        {
            JObject parameters = new JObject()
            {
                { "prop_handle", itemHandle }
            };

            HandleRequest("disable_property", parameters);
        }

        public void DisablePropertySerialization(JObject itemHandle)
        {
            JObject parameters = new JObject()
            {
                { "prop_handle", itemHandle }
            };

            HandleRequest("disable_property_serialization", parameters);
        }

        public void EnableProperty(JObject propHandle)
        {
            JObject parameters = new JObject()
            {
                { "prop_handle", propHandle }
            };

            HandleRequest("enable_property", parameters);
        }

        public void EnablePropertySerialization(JObject itemHandle)
        {
            JObject parameters = new JObject()
            {
                { "prop_handle", itemHandle }
            };

            HandleRequest("enable_property_serialization", parameters);
        }

        public bool IsPropertyEnabled(JObject propHandle)
        {
            JObject parameters = new JObject()
            {
                { "prop_handle", propHandle}
            };

            return (bool)HandleRequest("is_property_enabled", parameters)["result"]!;
        }

        public bool IsPropertySerializable(JObject propHandle)
        {
            JObject parameters = new JObject()
            {
                { "prop_handle", propHandle}
            };

            return (bool)HandleRequest("is_property_serializable", parameters)["result"]!;
        }

        public void Error(string msg, string kind = ErrorType.General, JObject? context = null)
        {
            JObject parameters = new JObject()
            {
                {"msg",msg },
                {"kind", kind},
                {"context", context}
            };

            HandleRequest("error", parameters);
        }

        public bool Exists(string name, JObject? parent = null, string itemType = ItemType.ANY)
        {
            JObject parameters = new JObject()
            {
                { "name", name },
                { "parent", parent},
                { "item_type", itemType}
            };

            return (bool)HandleRequest("exists", parameters)["result"]!;
        }

        public void ExportModelToJson(string? outputDir = null, string recursionStrategy = RecursionStrategy.None)
        {
            JObject parameters = new JObject()
            {
                { "output_dir", outputDir },
                { "recursion_strategy", recursionStrategy }
            };

            HandleRequest("export_model_to_json", parameters);
        }

        public List<JObject> FindConnections(JObject connectableHandle1, JObject? connectableHandle2 = null)
        {
            JObject parameters = new JObject()
            {
                { "connectable_handle1", connectableHandle1 },
                { "connectable_handle2", connectableHandle2 }
            };

            return ((JArray)HandleRequest("find_connections", parameters)["result"]!).Select(item => (JObject)item).ToList();
        }

        public static string Fqn(params string[] args)
        {
            IEnumerable<string> parts = args.SelectMany(arg => arg.Split(new[] { FqnSep }, StringSplitOptions.None)).Skip(1) // Ask why
                        .Where(p => !string.IsNullOrWhiteSpace(p));
            return string.Join(FqnSep, parts);
        }

        public List<string> GetAvailableLibraryComponents(string libraryName = "")
        {
            JObject parameters = new JObject()
            {
                { "library_name",  libraryName },
            };

            return ((JArray)HandleRequest("get_available_library_components", parameters)["result"]!).Select(item => (string)item!).ToList();
        }

        public List<Position> GetBreakpoints(JObject itemHandle)
        {
            JObject parameters = new JObject()
            {
                { "item_handle", itemHandle },
            };

            return ((JArray)HandleRequest("get_breakpoints", parameters)["result"]!).Select(coordinates => new Position((double)coordinates[0]!, (double)coordinates[1]!)).ToList();
        }

        public string GetCommentText(JObject commentHandle)
        {
            JObject parameters = new JObject()
            {
                { "comment_handle", commentHandle },
            };

            return (string)HandleRequest("get_comment_text", parameters)["result"]!;
        }

        public string GetCompiledModelFile(string schPath)
        {
            JObject parameters = new JObject()
            {
                { "sch_path", schPath },
            };

            return (string)HandleRequest("get_compiled_model_file", parameters)["result"]!;
        }

        public JObject GetComponentTypeName(JObject compHandle)
        {
            var parameters = new JObject()
            {
                { "comp_handle", compHandle },
            };

            return (JObject)HandleRequest("get_component_type_name", parameters)["result"]!;
        }

        public string GetConnectableDirection(JObject connectableHandle)
        {
            var parameters = new JObject()
            {
                { "connectable_handle", connectableHandle },
            };

            return (string)HandleRequest("get_connectable_direction", parameters)["result"]!;
        }

        public string GetConnectableKind(JObject connectableHandle)
        {
            var parameters = new JObject()
            {
                { "connectable_handle", connectableHandle },
            };

            return (string)HandleRequest("get_connectable_kind", parameters)["result"]!;
        }

        public List<JObject> GetConnectedItems(JObject itemHandle)
        {
            var parameters = new JObject()
            {
                { "item_handle", itemHandle },
            };

            return ((JArray)HandleRequest("get_connected_items", parameters)["result"]!).ToObject<List<JObject>>()!;
        }

        public string GetName(JObject itemHandle)
        {
            var parameters = new JObject()
            {
                { "item_handle", itemHandle },
            };

            return (string)HandleRequest("get_name", parameters)["result"]!;
        }

        public string GetConvProp(JObject propHandle, string? value = null) // ask more about solution
        {
            var parameters = new JObject()
            {
                { "prop_handle", propHandle },
                { "value", value },
            };

            return (string)HandleRequest("get_conv_prop", parameters)["result"]!;
        }

        public string GetDescription(JObject itemHandle)
        {
            var parameters = new JObject()
            {
                { "item_handle", itemHandle },
            };

            return (string)HandleRequest("get_description", parameters)["result"]!;
        }

        public string GetFqn(JObject itemHandle)
        {
            var parameters = new JObject()
            {
                { "item_handle", itemHandle },
            };

            return (string)HandleRequest("get_fqn", parameters)["result"]!;
        }

        public string GetHandlerCode(JObject itemHandle, string handlerName)
        {
            var parameters = new JObject()
            {
                { "item_handle", itemHandle },
                { "handler_name", handlerName },
            };

            return (string)HandleRequest("get_handler_code", parameters)["result"]!;
        }

        public void SetHandlerCode(JObject itemHandle, string handlerName, string code)
        {
            var parameters = new JObject()
            {
                { "item_handle", itemHandle },
                { "handler_name", handlerName },
                { "code", code },
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
            var parameters = new JObject()
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
            var parameters = new JObject()
            {
                { "prop_name", propName },
            };

            return (string)HandleRequest("get_hw_property", parameters)["result"]!;
        }

        public JObject? GetHWSettings()
        {
            try
            {
                JObject res = HandleRequest("get_hw_settings", new());
                return (JObject)res["result"]!;
            }
            catch (SchematicAPIException)
            {
                return null;
            }
            throw new UnreachableException();
        }

        public string GetIconDrawingCommands(JObject itemHandle)
        {
            var parameters = new JObject()
            {
                { "item_handle", itemHandle },
            };

            return (string)HandleRequest("get_icon_drawing_commands", parameters)["result"]!;
        }

        public void SetIconDrawingCommands(JObject itemHandle, string drawingCommands)
        {
            var parameters = new JObject()
            {
                { "item_handle", itemHandle },
                { "drawing_commands", drawingCommands },
            };

            HandleRequest("set_icon_drawing_commands", parameters);
        }

        public List<JObject> GetItems(JObject? parent = null, string itemType = ItemType.ANY)
        {
            var parameters = new JObject()
            {
                { "parent", parent },
                { "item_type", itemType },
            };

            return ((JArray)HandleRequest("get_items", parameters)["result"]!).ToObject<List<JObject>>()!;
        }

        public JObject CreateTag(string value, string? name = null, JObject? parent = null,
            string scope = "global", string kind = Kind.Pe, string? direction = null,
            string rotation = Rotation.Up, string flip = Flip.None, Position? position = null)
        {
            var parameters = new JObject()
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
            var parameters = new JObject()
            {
                { "item_handle", itemHandle },
            };

            return (string)HandleRequest("get_label", parameters)["result"]!;
        }

        public string GetLibraryResourceDirPath(JObject itemHandle)
        {
            var parameters = new JObject()
            {
                { "item_handle", itemHandle },
            };

            return (string)HandleRequest("get_library_resource_dir_path", parameters)["result"]!;
        }

        public JObject GetMask(JObject itemHandle)
        {
            var parameters = new JObject()
            {
                { "item_handle", itemHandle },
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
            return (string)HandleRequest("get_model_file_path", new())["result"]!;
        }

        public JObject GetModelInformation()
        {
            return (JObject)HandleRequest("get_model_information", new())["result"]!;
        }

        public string GetModelPropertyValue(string propCodeName) // consider another solution for dynamic types
        {
            var parameters = new JObject()
            {
                { "prop_code_name", propCodeName },
            };

            return (string)HandleRequest("get_model_property_value", parameters)["result"]!;
        }

        public string GetNamespaceVariable(string varName)
        {
            var parameters = new JObject()
            {
                { "var_name", varName },
            };

            return (string)HandleRequest("get_ns_var", parameters)["result"]!;
        }

        public JObject GetNamespaceVariables()
        {
            return (JObject)HandleRequest("get_ns_vars", new())["result"]!;
        }

        public void SetNamespaceVariable(string varName, object value)
        {
            var parameters = new JObject()
            {
                { "var_name", varName },
                { "value", JToken.FromObject(value) },
            };

            HandleRequest("set_ns_var", parameters);
        }

        public JObject GetParent(JObject itemHandle)
        {
            var parameters = new JObject()
            {
                { "item_handle", itemHandle },
            };

            return (JObject)HandleRequest("get_parent", parameters)["result"]!;
        }

        public Position GetPosition(JObject itemHandle)
        {
            var parameters = new JObject()
            {
                { "item_handle", itemHandle },
            };
            var res = (JArray)HandleRequest("get_position", parameters)["result"]!;
            return new Position((double)res[0], (double)res[1]);
        }

        public void SetPosition(JObject itemHandle, Position position)
        {
            var parameters = new JObject()
            {
                { "item_handle", itemHandle },
                { "position", position.JArray },
            };

            HandleRequest("set_position", parameters);
        }

        public List<string> GetPropertyComboValues(JObject propHandle)
        {
            var parameters = new JObject()
            {
                { "prop_handle", propHandle },
            };

            return ((JArray)HandleRequest("get_property_combo_values", parameters)["result"]!).ToObject<List<string>>()!;
        }

        public void SetPropertyComboValues(JObject propHandle, List<string> comboValues)
        {
            var parameters = new JObject()
            {
                { "prop_handle", propHandle },
                { "combo_values", new JArray(comboValues) },
            };

            HandleRequest("set_property_combo_values", parameters);
        }

        public string GetPropertyDefaultValue(JObject propHandle)
        {
            var parameters = new JObject()
            {
                { "prop_handle", propHandle },
            };

            return (string)HandleRequest("get_property_default_value", parameters)["result"]!;
        }

        public string GetPropertyDisplayValue(JObject propHandle)
        {
            var parameters = new JObject()
            {
                { "prop_handle", propHandle },
            };

            return (string)HandleRequest("get_property_disp_value", parameters)["result"]!;
        }

        public void SetPropertyDisplayValue(JObject propHandle, string value)
        {
            var parameters = new JObject()
            {
                { "prop_handle", propHandle },
                { "value", value },
            };

            HandleRequest("set_property_disp_value", parameters);
        }

        public JObject GetPropertyTypeAttributes(JObject propHandle)
        {
            var parameters = new JObject()
            {
                { "prop_handle", propHandle },
            };

            return (JObject)HandleRequest("get_property_type_attributes", parameters)["result"]!;
        }

        public string GetPropertyValue(JObject propHandle)
        {
            var parameters = new JObject()
            {
                { "prop_handle", propHandle },
            };

            return (string)HandleRequest("get_property_value", parameters)["result"]!;
        }


    }

}
