using System.Text.Json.Nodes;

namespace HutchAgent.Utilities;

public class ConfigPropsParsingUtility
{
  private readonly IConfigurationRoot _configurationRoot;

  public ConfigPropsParsingUtility(IConfigurationRoot configurationRoot)
  {
    _configurationRoot = configurationRoot;
  }

  public JsonObject GetObject(string sectionPath)
  {
    var result = new JsonObject();

    var section = _configurationRoot.GetSection(sectionPath);
    var sectionDict = section.AsEnumerable().ToDictionary(c => c.Key, c => c.Value);

    foreach (var (key, value) in sectionDict)
    {
      result[key] = value;
    }

    return result;
  }
}
