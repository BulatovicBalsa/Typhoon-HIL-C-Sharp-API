using Newtonsoft.Json.Linq;
using System.Reflection.Metadata;

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

    internal class SchematicAPI : AbsractAPI
    {
        public SchematicAPI() { }
        public SchematicAPI(ICommunication communication) : base(communication) { }

        public override int ProperPort => Ports.SchematicApiPort;

        public JObject Load(string filename)
        {
            return Request("load", new JObject() { { "filename", filename } });
        }

        public JObject Compile()
        {
            return Request("compile", new());
        }

        public JObject CreateNewModel()
        {
            return Request("create_new_model", new());
        }

        public JObject CloseModel()
        {
            return Request("close_model", new());
        }

        public JObject SaveAs(string filename)
        {
            return Request("save_as", new JObject() { { "filename", filename } });
        }

        public JObject GetLibraryPaths()
        {
            return Request("get_library_paths", new());
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

            return (JObject)Request("create_component", parameters)["result"]!;
        }

        public JObject CreateJunction(string? name=null, JObject? parent = null, string kind = Kind.Pe, Position? position = null)
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

        public JObject CreateConnection(JObject start,  JObject end, string? name = null) //check what breakpoints are
        {
            var parameters = new JObject()
            {
                { "name", name },
                { "start", start },
                { "end", end },
                { "breakpoints", null },
            };

            return Request("create_connection", parameters);
        }
    }

}
