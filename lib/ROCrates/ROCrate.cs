using ROCrates.Models;

namespace ROCrates;
public class ROCrate
{
  private List<ContextEntity> _contextEntities = new();
  private List<DataEntity> _dataEntities = new();
  private List<Entity> _defaultEntities = new();
  private Dictionary<string, Entity> _entityMap = new();
  private string _uuid = Guid.NewGuid().ToString();
  /// TODO: add the following fields:
  ///   - preview (based on Preview class not yet made)
  ///   - arcp_base_uri ($"arcp://uuid,{_uuid}")

  private string _source;
  private List<string>? _exclude;
  private bool _generatePreview;
  private bool _init;

  public ROCrate(string source, bool generatePreview = false, bool init = false, List<string>? exclude = null)
  {
    _source = source;
    _generatePreview = generatePreview;
    _init = init;
    _exclude = exclude;
  }
}
