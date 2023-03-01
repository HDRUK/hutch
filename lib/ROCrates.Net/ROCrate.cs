using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using ROCrates.Models;
using File = ROCrates.Models.File;

namespace ROCrates;
public class ROCrate
{
  private List<ContextEntity> _contextEntities = new();
  private List<DataEntity> _dataEntities = new();
  private List<Entity> _defaultEntities = new();
  private static string _uuid = Guid.NewGuid().ToString();
  private string _arcpBaseUri = $"arcp://uuid,{_uuid}/";
  /// TODO: add the following fields:
  ///   - preview (based on Preview class not yet made)

  private string _source = string.Empty;
  private List<string> _exclude = new();
  private bool _generatePreview;
  private bool _init;

  public RootDataset? RootDataset { get; set; }
  
  public Metadata? Metadata { get; set; }
  
  public Dictionary<string, Entity> Entities = new();

  /// <summary>
  /// Initialise a new empty <c>ROCrate</c> object. This constructor will not create or parse an RO-Crate on disk.
  /// </summary>
  public ROCrate()
  {
  }

  public ROCrate(string source, bool generatePreview = false, bool init = false, List<string>? exclude = null)
  {
    _source = source;
    _generatePreview = generatePreview;
    _init = init;
    if (exclude is not null) _exclude = exclude;
  }

  /// <summary>
  /// Resolves URI for a given ID.
  /// </summary>
  /// <param name="id">The ID to be resolved.</param>
  /// <returns>The resolved URI for the given ID.</returns>
  public string ResolveId(string id)
  {
    if (Uri.IsWellFormedUriString(id, UriKind.Absolute)) return id.TrimEnd('/');

    var resolvedId = Path.Combine(_arcpBaseUri, id);
    return resolvedId.TrimEnd('/');
  }

  /// <summary>
  /// Add entities to an <c>ROCrate</c>.
  /// </summary>
  /// <example>
  /// <code>
  /// var roCrate = new ROCrate();
  /// var textFile = new File(roCrate, source: "my-file.txt");
  /// var imageFile = new File(roCrate, source: "my-image.png");
  /// var person = new Person();
  /// roCrate.Add(textFile, imageFile, person);
  /// </code>
  /// </example>
  /// <param name="entities">The entities to add the the <c>ROCrate</c>.</param>
  public void Add(params Entity[] entities)
  {
    foreach (var entity in entities)
    {
      var entityType = entity.GetType();
      var key = entity.GetCanonicalId();
      if (entityType == typeof(RootDataset))
      {
        RootDataset = entity as RootDataset;
      }
      if (entityType == typeof(Metadata))
      {
        Metadata = entity as Metadata;
      }
      Entities.Add(key, entity);
    }
  }
}
