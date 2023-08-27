using NetMQ;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Security.Policy;
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
            var parameters = new JObject() {
                { "type_name", typeName },
                {"parent", parent },
                {"name", name },
                {"rotation", rotation},
                {"flip", flip},
                {"hide_name", hideName },
                {"position", position?.JArray },
                {"size", size?.JArray }
            };

            var res = HandleRequest("create_component", parameters);
            return (JObject)res["result"]!;
        }

        public JObject CreateJunction(string? name = null, JObject? parent = null, string kind = Kind.Pe, Position? position = null)
        {
            var parameters = new JObject()
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
            var parameters = new JObject()
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
            var parameters = new JObject()
            {
                { "term_name", terminalName },
                { "comp_handle", componentHandle },
            };
            return (JObject)Request("term", parameters)["result"]!;
        }

        public JObject CreateConnection(JObject start, JObject end, string? name = null, List<Position>? breakpoints = null) //check what breakpoints are
        {
            var parameters = new JObject()
            {
                { "name", name },
                { "start", start },
                { "end", end },
                { "breakpoints", new JArray() { breakpoints?.Select(bp => bp.JArray) } },
            };

            return (JObject)HandleRequest("create_connection", parameters)["result"]!;
        }

        public JObject SetPropertyValue(JObject propertyHandle, object value)
        {
            var parameters = new JObject()
            {
                { "prop_handle", propertyHandle },
                { "value", JToken.FromObject(value) },
            };

            return Request("set_property_value", parameters);
        }

        public JObject Prop(JObject itemHandle, string propertyName)
        {
            var parameters = new JObject()
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
            var parameters = new JObject()
            {
                { "library_path", libraryPath },
                { "add_subdirs", addSubdirs },
                { "persist", persist },
            };

            return Request("add_library_path", parameters);
        }

        public JObject RemoveLibraryPath(string libraryPath, bool persist = false)
        {
            var parameters = new JObject()
            {
                { "library_path", libraryPath },
                { "persist", persist },
            };

            return Request("remove_library_path", parameters);
        }

        public JObject CreateComment(string text, JObject? parent = null, string? name = null, Position? position = null)
        {
            var parameters = new JObject()
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
            var parameters = new JObject()
            {
                { "lib_name", libraryName },
                { "file_name", fileName },
            };

            HandleRequest("create_library_model", parameters);
        }

        public JObject CreateMask(JObject itemHandle)
        {
            var parameters = new JObject()
            {
                { "item_handle", itemHandle },
            };

            var res = HandleRequest("create_mask", parameters);
            return (JObject)res["result"]!;
        }

        public void DeleteItem(JObject itemHandle)
        {
            var parameters = new JObject()
            {
                { "item_handle", itemHandle },
            };

            HandleRequest("delete_item", parameters);
        }

        public JObject? DetectHwSettings() // Consider creating new Model with hw configuration
        {
            try
            {
                var res = HandleRequest("detect_hw_settings", new());
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
            var parameters = new JObject()
            {
                { "item_handles", new JArray() { itemHandles } }
            };

            var res = HandleRequest("disable_items", parameters);
            return ((JArray)res["result"]!).Select(item => (JObject)item).ToList();
        }
        
        public List<JObject> EnableItems(List<JObject?> itemHandles)
        {
            var parameters = new JObject()
            {
                { "item_handles", new JArray() { itemHandles } }
            };

            var res = HandleRequest("enable_items", parameters);
            return ((JArray)res["result"]!).Select(item => (JObject)item).ToList();
        }

        public bool IsEnabled(JObject itemHandle)
        {
            var parameters = new JObject()
            {
                { "item_handle", itemHandle }
            };

            return (bool)HandleRequest("is_enabled", parameters)["result"]!;
        }

        public JObject? GetItem(string name, JObject? parent = null, string itemType = ItemType.ANY)
        {
            var parameters = new JObject()
            {
                { "name", name },
                { "item_type", itemType },
                { "parent", parent },
            };

            return HandleRequest("get_item", parameters)["result"]?.Value<JObject>();
        }

        public void DisableProperty(JObject itemHandle)
        {
            var parameters = new JObject()
            {
                { "prop_handle", itemHandle }
            };

            HandleRequest("disable_property", parameters);
        }

        public void DisablePropertySerialization(JObject itemHandle)
        {
            var parameters = new JObject()
            {
                { "prop_handle", itemHandle }
            };

            HandleRequest("disable_property_serialization", parameters);
        }

        public void EnableProperty(JObject propHandle)
        {
            var parameters = new JObject()
            {
                { "prop_handle", propHandle }
            };

            HandleRequest("enable_property", parameters);
        }

        public void EnablePropertySerialization(JObject itemHandle)
        {
            var parameters = new JObject()
            {
                { "prop_handle", itemHandle }
            };

            HandleRequest("enable_property_serialization", parameters);
        }

        public bool IsPropertyEnabled(JObject propHandle)
        {
            var parameters = new JObject()
            {
                { "prop_handle", propHandle}
            };

            return (bool)HandleRequest("is_property_enabled", parameters)["result"]!;
        }

        public bool IsPropertySerializable(JObject propHandle)
        {
            var parameters = new JObject()
            {
                { "prop_handle", propHandle}
            };

            return (bool)HandleRequest("is_property_serializable", parameters)["result"]!;
        }

        public void Error(string msg, string kind = ErrorType.General, JObject? context= null)
        {
            var parameters = new JObject()
            {
                {"msg",msg },
                {"kind", kind},
                {"context", context}
            };

            HandleRequest("error", parameters);
        }

        public bool Exists(string name, JObject? parent = null, string itemType = ItemType.ANY)
        {
            var parameters = new JObject()
            {
                { "name", name },
                { "parent", parent},
                { "item_type", itemType}
            };

            return (bool)HandleRequest("exists", parameters)["result"]!;
        }

        public void ExportModelToJson(string? outputDir = null, string recursionStrategy = RecursionStrategy.None)
        {
            var parameters = new JObject()
            {
                { "output_dir", outputDir },
                { "recursion_strategy", recursionStrategy }
            };

            HandleRequest("export_model_to_json", parameters);
        }

        public List<JObject> FindConnections(JObject connectableHandle1, JObject? connectableHandle2 = null)
        {
            var parameters = new JObject()
            {
                { "connectable_handle1", connectableHandle1 },
                { "connectable_handle2", connectableHandle2 }
            };

            return ((JArray)HandleRequest("find_connections", parameters)["result"]!).Select(item => (JObject)item).ToList();
        }

        public static string Fqn(params string[] args)
        {
            var parts = args.SelectMany(arg => arg.Split(new[] { FqnSep }, StringSplitOptions.None)).Skip(1) // Ask why
                        .Where(p => !string.IsNullOrWhiteSpace(p));
            return string.Join(FqnSep, parts);
        }

        public List<string> GetAvailableLibraryComponents(string libraryName="") 
        {
            var parameters = new JObject()
            {
                { "library_name",  libraryName },
            };

            return ((JArray)HandleRequest("get_available_library_components", parameters)["result"]!).Select(item => (string)item!).ToList();
        }

        public List<Position> GetBreakpoints(JObject itemHandle)
        {
            var parameters = new JObject()
            {
                { "item_handle", itemHandle },
            };

            return ((JArray)HandleRequest("get_breakpoints", parameters)["result"]!).Select(coordinates => new Position((double)coordinates[0]!, (double)coordinates[1]!)).ToList();
        }
    }

}
