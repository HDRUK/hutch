using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json.Nodes;
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

  public RootDataset RootDataset;

  public Metadata Metadata;
  
  public Dictionary<string, Entity> Entities = new();

  /// <summary>
  /// Initialise a new empty <c>ROCrate</c> object. This constructor will not create or parse an RO-Crate on disk.
  /// </summary>
  public ROCrate()
  {
    RootDataset = new RootDataset(this);
    Metadata = new Metadata(this);
  }

  public ROCrate(string source, bool generatePreview = false, bool init = false, List<string>? exclude = null)
  {
    _source = source;
    _generatePreview = generatePreview;
    _init = init;
    if (exclude is not null) _exclude = exclude;
    RootDataset = new RootDataset(this);
    Metadata = new Metadata(this);
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
  /// var person = new Person(roCrate);
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
      if (entityType.IsSubclassOf(typeof(DataEntity)))
      {
        if (!Entities.ContainsKey(key)) RootDataset.AppendTo("hasPart", entity);
      }
      Entities.Add(key, entity);
    }
  }

  /// <summary>
  /// Add a person to the RO-Crate and return the created <c>Person</c> object.
  /// </summary>
  /// <example>
  /// <code>
  /// var roCrate = new ROCrate();
  /// var person = roCrate.AddPerson();
  /// </code>
  /// </example>
  /// <param name="identifier">The unique identifier of the person.</param>
  /// <param name="properties">Additional properties of the person.</param>
  /// <remarks>A new <c>Person</c> with the given parameters.</remarks>
  public Person AddPerson(string? identifier = null, JsonObject? properties = null)
  {
    var person = new Person(this, identifier, properties);
    Add(person);
    return person;
  }

  /// <summary>
  /// Add a file to the RO-Crate and return the created <c>File</c> object.
  /// </summary>
  /// <example>
  /// <code>
  /// var roCrate = new ROCrate();
  /// var file = roCrate.AddFile();
  /// </code>
  /// </example>
  /// <param name="identifier">The unique identifier.</param>
  /// <param name="properties">Additional properties of the file.</param>
  /// <param name="source">The path to the file.</param>
  /// <param name="destPath">The path to where file will be saved.</param>
  /// <param name="fetchRemote">Fetch the file from remote location?</param>
  /// <param name="validateUrl">Check the URL?</param>
  /// <returns>A new <c>File</c> with the given parameters.</returns>
  public File AddFile(string? identifier = null, JsonObject? properties = null, string? source = null,
    string? destPath = null, bool fetchRemote = false, bool validateUrl = false)
  {
    var file = new File(this, identifier, properties, source, destPath, fetchRemote, validateUrl);
    Add(file);
    return file;
  }
  
  /// <summary>
  /// Add a dataset to the RO-Crate and return the created <c>Dataset</c> object.
  /// </summary>
  /// <example>
  /// <code>
  /// var roCrate = new ROCrate();
  /// var dataset = roCrate.AddDataset();
  /// </code>
  /// </example>
  /// <param name="identifier">The unique identifier.</param>
  /// <param name="properties">Additional properties of the dataset.</param>
  /// <param name="source">The path to the dataset.</param>
  /// <param name="destPath">The path to where dataset will be saved.</param>
  /// <param name="fetchRemote">Fetch the dataset from remote location?</param>
  /// <param name="validateUrl">Check the URL?</param>
  /// <returns>A new <c>Dataset</c> with the given parameters.</returns>
  public Dataset AddDataset(string? identifier = null, JsonObject? properties = null, string? source = null,
    string? destPath = null, bool fetchRemote = false, bool validateUrl = false)
  {
    var dataset = new Dataset(this, identifier, properties, source, destPath, fetchRemote, validateUrl);
    Add(dataset);
    return dataset;
  }
}
