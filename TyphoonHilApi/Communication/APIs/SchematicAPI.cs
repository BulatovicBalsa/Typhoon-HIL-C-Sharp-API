using Newtonsoft.Json.Linq;

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

            return Request("create_component", parameters);
        }
    }
}
