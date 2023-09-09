using Newtonsoft.Json.Linq;
using TyphoonHil.Communication;
using TyphoonHil.Exceptions;

namespace TyphoonHil.API;

public class ConfigurationManagerAPI : AbstractAPI
{
    public ConfigurationManagerAPI()
    {
    }

    internal ConfigurationManagerAPI(ICommunication communication) : base(communication)
    {
    }

    protected override int ProperPort => Ports.ConfigurationManagerApiPort;

    public JObject LoadProject(string projectPath)
    {
        var parameters = new JObject
        {
            { "project_path", projectPath }
        };

        return (JObject)HandleRequest("load_project", parameters)["result"]!;
    }

    public JObject CreateConfig(string configName)
    {
        var parameters = new JObject
        {
            { "config_name", configName }
        };

        return (JObject)HandleRequest("create_config", parameters)["result"]!;
    }

    public JObject Generate(JObject projectHandle, JObject configHandle, string outDir = "", string fileName = "",
        bool standaloneModel = true)
    {
        var parameters = new JObject
        {
            { "project_handle", projectHandle },
            { "config_handle", configHandle },
            { "out_dir", outDir },
            { "file_name", fileName },
            { "standalone_model", standaloneModel }
        };

        return (JObject)HandleRequest("generate", parameters)["result"]!;
    }

    public string GetName(JObject itemHandle)
    {
        var parameters = new JObject
        {
            { "item_handle", itemHandle }
        };

        return (string)HandleRequest("get_name", parameters)["result"]!;
    }

    public JObject MakePick(string variantName, string optionName, JObject? optionConfiguration = null)
    {
        var parameters = new JObject
        {
            { "variant_name", variantName },
            { "option_name", optionName },
            { "option_configuration", optionConfiguration }
        };

        return (JObject)HandleRequest("make_pick", parameters)["result"]!;
    }

    public void Picks(JObject configHandle, List<JObject> pickHandles)
    {
        var configHandleArray = new JArray(pickHandles.Select(handle => handle));

        var parameters = new JObject
        {
            { "config_handle", configHandle },
            { "pick_handles", configHandleArray }
        };

        HandleRequest("picks", parameters);
    }

    public JObject LoadConfig(string configPath)
    {
        var parameters = new JObject { { "config_path", configPath } };
        return (JObject)HandleRequest("load_config", parameters)["result"]!;
    }

    public List<JObject> GetOptions(JObject projectHandle, JObject variantHandle)
    {
        var parameters = new JObject
        {
            { "project_handle", projectHandle },
            { "variant_handle", variantHandle }
        };

        return ((JArray)HandleRequest("get_options", parameters)["result"]!).Select(item => (JObject)item).ToList();
    }

    public List<JObject> GetProjectVariants(JObject projectHandle)
    {
        var parameters = new JObject
        {
            { "project_handle", projectHandle }
        };

        return ((JArray)HandleRequest("get_project_variants", parameters)["result"]!).Select(item => (JObject)item)
            .ToList();
    }

    public void SaveConfig(JObject configHandle, string savePath)
    {
        var parameters = new JObject
        {
            { "config_handle", configHandle },
            { "save_path", savePath }
        };

        HandleRequest("save_config", parameters);
    }

    protected override JObject HandleRequest(string method, JObject parameters)
    {
        var res = Request(method, parameters);
        if (!res.ContainsKey("error")) return res;
        var msg = (string)res["error"]!["message"]!;
        throw new ConfigurationManagerAPIException(msg);
    }
}