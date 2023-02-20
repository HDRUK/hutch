using System.Text.Encodings.Web;
using ROCrates.Models;

namespace ROCrates;
public class ROCrate
{
  private List<ContextEntity> _contextEntities = new();
  private List<DataEntity> _dataEntities = new();
  private List<Entity> _defaultEntities = new();
  private Dictionary<string, Entity> _entityMap = new();
  private static string _uuid = Guid.NewGuid().ToString();
  private string _arcpBaseUri = $"arcp://uuid,{_uuid}/";
  /// TODO: add the following fields:
  ///   - preview (based on Preview class not yet made)

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

  /// <summary>
  /// Resolves URI for a given ID.
  /// </summary>
  /// <param name="id">The ID to be resolved.</param>
  /// <returns>The resolved URI for the given ID.</returns>
  public string ResolveId(string id)
  {
    if (Uri.IsWellFormedUriString(id, UriKind.Absolute)) return id.TrimEnd('/');
    
    Uri.TryCreate(new Uri(_arcpBaseUri), id, out var newUri);
    return newUri.ToString().TrimEnd('/');
  }
}
