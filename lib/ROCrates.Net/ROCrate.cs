using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Nodes;
using ROCrates.Exceptions;
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

  public RootDataset RootDataset;
  public Metadata Metadata;
  public Preview Preview;

  public Dictionary<string, Entity> Entities = new();
  private Entity? _mainEntity = null;

  /// <summary>
  /// Initialise a new empty <c>ROCrate</c> object. This constructor will not create or parse an RO-Crate on disk.
  /// </summary>
  public ROCrate()
  {
    Metadata = new Metadata(this);
    Add(Metadata);

    Preview = new Preview(this);
    Add(Preview);

    RootDataset = new RootDataset(this);
    Add(RootDataset);
  }

  /// <summary>
  /// Resolves URI for a given ID.
  /// </summary>
  /// <param name="id">The ID to be resolved.</param>
  /// <returns>The resolved URI for the given ID.</returns>
  public string ResolveId(string id)
  {
    if (Uri.IsWellFormedUriString(id, UriKind.RelativeOrAbsolute)) return id;

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
  /// <param name="entities">The entities to add the RO-Crate.</param>
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

      else if (entityType == typeof(Metadata))
      {
        Metadata = entity as Metadata;
        _dataEntities.Add(entity as Metadata);
      }

      else if (entityType == typeof(Preview))
      {
        Preview = entity as Preview;
        _dataEntities.Add(entity as Preview);
      }

      else if (entityType.IsSubclassOf(typeof(DataEntity)))
      {
        if (!Entities.ContainsKey(key)) RootDataset.AppendTo("hasPart", entity);
        _dataEntities.Add(entity as DataEntity);
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

  /// <summary>
  /// Add a workflow to the RO-Crate and return the created <c>ComputationalWorkflow</c> object.
  /// </summary>
  /// <example>
  /// <code>
  /// var roCrate = new ROCrate();
  /// var workflow = roCrate.AddWorkflow();
  /// </code>
  /// </example>
  /// <param name="identifier">The unique identifier.</param>
  /// <param name="properties">Additional properties of the workflow.</param>
  /// <param name="source">The path to the workflow file.</param>
  /// <param name="destPath">The path to where workflow file will be saved.</param>
  /// <param name="fetchRemote">Fetch the workflow from remote location?</param>
  /// <param name="validateUrl">Check the URL?</param>
  /// <returns>A new <c>ComputationalWorkflow</c> with the given parameters.</returns>
  public ComputationalWorkflow AddWorkflow(string? identifier = null, JsonObject? properties = null,
    string? source = null, string? destPath = null, bool fetchRemote = false, bool validateUrl = false)
  {
    var workflow = new ComputationalWorkflow(this, identifier, properties, source, destPath, fetchRemote, validateUrl);

    var profiles = Metadata.GetProperty<List<Part>>("conformsTo") ?? new List<Part>();
    profiles.Add(new Part { Id = "https://w3id.org/workflowhub/workflow-ro-crate/1.0" });
    Metadata.SetProperty("conformsTo", profiles);

    Add(workflow);
    return workflow;
  }

  /// <summary>
  /// Add a test definition to the RO-Crate and return the created <c>TestDefinition</c> object.
  /// </summary>
  /// <example>
  /// <code>
  /// var roCrate = new ROCrate();
  /// var testDefinition = roCrate.AddTestDefinition();
  /// </code>
  /// </example>
  /// <param name="identifier">The unique identifier.</param>
  /// <param name="properties">Additional properties of the test definition.</param>
  /// <param name="source">The path to the test definition file.</param>
  /// <param name="destPath">The path to where test definition file will be saved.</param>
  /// <param name="fetchRemote">Fetch the test definition from remote location?</param>
  /// <param name="validateUrl">Check the URL?</param>
  /// <returns>A new <c>TestDefinition</c> with the given parameters.</returns>
  public TestDefinition AddTestDefinition(string? identifier = null, JsonObject? properties = null,
    string? source = null, string? destPath = null, bool fetchRemote = false, bool validateUrl = false)
  {
    var testDefinition = new TestDefinition(this, identifier, properties, source, destPath, fetchRemote, validateUrl);
    Add(testDefinition);
    return testDefinition;
  }

  /// <summary>
  /// Add a test suite to the RO-Crate and return the created <c>TestSuite</c> object.
  /// </summary>
  /// <example>
  /// <code>
  /// var roCrate = new ROCrate();
  /// var testSuite = roCrate.AddTestSuite();
  /// </code>
  /// </example>
  /// <param name="identifier">The identifier of the test suite.</param>
  /// <param name="name">The name of the test suite.</param>
  /// <param name="mainEntity">The main entity of the test suite.</param>
  /// <returns>The <c>TestSuite</c> object with the given parameters.</returns>
  public TestSuite AddTestSuite(string? identifier = null, string? name = null, Entity? mainEntity = null)
  {
    var testRefProp = "mentions";
    if (mainEntity is null && _mainEntity is null) testRefProp = "about";

    var suite = new TestSuite(this, identifier);
    suite.Name = name ?? suite.Id.TrimStart('#');

    if (mainEntity is not null) suite.SetProperty("mainEntity", mainEntity);
    else if (_mainEntity is not null) suite.SetProperty("mainEntity", _mainEntity);

    RootDataset.AppendTo(testRefProp, suite);
    Metadata.ExtraTerms = JsonSerializer.SerializeToNode(new TestingExtraTerms()).AsObject();
    Add(suite);
    return suite;
  }

  /// <summary>
  /// Add a test instance to the RO-Crate and return the <see cref="TestInstance"/> object.
  /// </summary>
  /// <example>
  /// <code>
  /// var roCrate = new ROCrate();
  /// var testInstance = roCrate.AddTestInstance();
  /// </code>
  /// </example>
  /// <param name="testSuite">The suite the test instance is run in.</param>
  /// <param name="url">The URL to the test instance.</param>
  /// <param name="resource">The resource of the test instance.</param>
  /// <param name="testService">The service used to run the test instance.</param>
  /// <param name="name">The name of the test instance.</param>
  /// <returns>The <see cref="TestInstance"/> with the given parameters.</returns>
  public TestInstance AddTestInstance(TestSuite testSuite, string url, string resource = "",
    TestService? testService = null, string? name = null)
  {
    var testInstance = new TestInstance(this);
    testInstance.SetProperty("url", url);
    testInstance.SetProperty("resource", resource);
    testInstance.Name = name ?? testInstance.Id.TrimStart('#');
    if (testService is not null) testInstance.RunsOn = testService;
    testSuite.AppendTo("instance", testInstance);
    Metadata.ExtraTerms = JsonSerializer.SerializeToNode(new TestingExtraTerms()).AsObject();
    Add(testInstance);
    return testInstance;
  }

  /// <summary>
  /// Save the RO-Crate to disk. 
  /// </summary>
  /// <param name="location">
  /// The directory where the data entities will be written. This will become a .zip file with the name
  /// {location}.zip if <c>zip</c> is <c>true</c>.
  /// </param>
  /// <param name="zip">
  /// If <c>true</c>, save the RO-Crate as a .zip file, else save to a directory. Default: <c>false</c>
  /// </param>
  public void Save(string? location = null, bool zip = false)
  {
    var saveLocation = location ?? Directory.GetCurrentDirectory();
    if (!Directory.Exists(saveLocation)) Directory.CreateDirectory(saveLocation);

    foreach (var entity in _dataEntities)
    {
      entity.Write(saveLocation);
    }

    if (!zip) return;
    ZipFile.CreateFromDirectory(saveLocation, $"{saveLocation}.zip");
    Directory.Delete(saveLocation, recursive: true);
  }

  /// <summary>
  /// <para>Initialise an <c>ROCrate</c> object from the contents of a directory that is a valid RO-Crate.</para>
  /// <para>
  /// This method assumes that all files and directories described by the metadata exist on disk.
  /// It will not fetch them from remote locations.
  /// </para>
  /// </summary>
  /// <param name="source">
  /// The path to the directory containing the RO-Crate to initialise an <c>ROCrate</c> from.
  /// </param>
  /// <exception cref="CrateReadException">Thrown when there is an issue reading the RO-Crate.</exception>
  /// <exception cref="MetadataException">Thrown when there is an issue with the RO-Crate's metadata.</exception>
  public void Initialise(string source)
  {
    try
    {
      CheckSourceMetadata(source);
    }
    catch (JsonException e)
    {
      throw new MetadataException("The RO-Crate metadata is invalid", e);
    }
    catch (InvalidDataException e)
    {
      throw new MetadataException("The RO-Crate metadata is invalid", e);
    }
    catch (Exception e)
    {
      throw new CrateReadException($"Could not read RO-Crate at {source}.", e);
    }

    // Read metadata and update the Metadata object
    var metadataPath = Path.Combine(source, "ro-crate-metadata.json");
    var json = System.IO.File.ReadAllText(metadataPath);
    var metadataJson = JsonNode.Parse(json);
    var graph = metadataJson!["@graph"]!.AsArray();
    var filteredGraph = from g in graph
      where g["@id"].ToString() == "ro-crate-metadata.json"
      select g;
    var metadataProperties = filteredGraph.First().AsObject();

    if (metadataProperties is null) throw new MetadataException("Could not find the metadata properties.");
    Metadata.Properties = metadataProperties;

    // Add objects to the crate
    foreach (var g in graph)
    {
      if (g?["@type"] is null || g["@id"] is null) throw new MetadataException("Invalid element in @graph.");
      var type = g["@type"]!.ToJsonString();
      switch (type)
      {
        case @"[""File"",""SoftwareSourceCode"",""ComputationalWorkflow""]":
          Add(new ComputationalWorkflow(this, source: g["@id"]!.ToString(), properties: g.AsObject()));
          break;
        case @"""ComputerLanguage""":
          Add(new ComputerLanguage(this, g["@id"]!.ToString(), g.AsObject()));
          break;
        case @"""CreativeWork""":
          switch (g["@id"]!.ToString())
          {
            case "ro-crate-metadata.json":
            {
              var metadata = new Metadata(this, source: g["@id"]!.ToString(), properties: g.AsObject());
              Add(metadata);
              break;
            }
            case "ro-crate-preview.html":
            {
              var preview = new Preview(this, source: g["@id"]!.ToString(), properties: g.AsObject());
              Add(preview);
              break;
            }
            default:
              Add(new CreativeWork(this, g["@id"]!.ToString(), g.AsObject()));
              break;
          }

          break;
        case @"""Dataset""":
          Add(g["@id"]!.ToString() == "./"
            ? new RootDataset(this, source: g["@id"]!.ToString(), properties: g.AsObject())
            : new Dataset(this, source: g["@id"]!.ToString(), properties: g.AsObject()));

          break;
        case @"""File""":
          Add(new File(this, source: g["@id"]!.ToString(), properties: g.AsObject()));
          break;
        case @"""Person""":
          Add(new Person(this, g["@id"]!.ToString(), g.AsObject()));
          break;
        case @"""SoftwareApplication""":
          Add(new SoftwareApplication(this, g["@id"]!.ToString(), g.AsObject()));
          break;
        case @"""TestDefinition""":
          Add(new TestDefinition(this, source: g["@id"]!.ToString(), properties: g.AsObject()));
          break;
        case @"""TestInstance""":
          Add(new TestInstance(this, g["@id"]!.ToString(), g.AsObject()));
          break;
        case @"""TestService""":
          Add(new TestService(this, g["@id"]!.ToString(), g.AsObject()));
          break;
        case @"""TestSuite""":
          Add(new TestSuite(this, g["@id"]!.ToString(), g.AsObject()));
          break;
        case @"[""File"",""SoftwareSourceCode"",""Workflow""]":
          Add(new Workflow(this, source: g["@id"]!.ToString(), properties: g.AsObject()));
          break;
        case @"[""File"",""SoftwareSourceCode"",""WorkflowDescription""]":
          Add(new WorkflowDescription(this, source: g["@id"]!.ToString(), properties: g.AsObject()));
          break;
        default:
          Add(new ContextEntity(this, g["@id"]!.ToString(), g.AsObject()));
          break;
      }
    }
  }

  /// <summary>
  /// <para>Convert a directory into an RO-Crate.</para>
  /// <para>
  /// This method iterates over the files and directories contained inside <paramref name="source"/>
  /// and populates an <c>ROCrate</c> object with entities representing the contents of the RO-Crate.
  /// The <c>ro-crate-metadata.json</c> and <c>ro-crate-preview.html</c> files are then saved into
  /// <paramref name="source"/>
  /// </para>
  /// </summary>
  /// <example>
  /// <code>
  /// var roCrate = new ROCrate();
  /// roCrate.Convert("convertMe");
  /// </code>
  /// </example>
  /// <param name="source">The path to the directory to be converted to an RO-Crate.</param>
  public void Convert(string source)
  {
    var dirInfo = new DirectoryInfo(source);

    // Add directories and files contained in those directories
    foreach (var dir in dirInfo.EnumerateDirectories("*", SearchOption.AllDirectories))
    {
      var dataset = AddDataset(source: Path.GetRelativePath(dirInfo.FullName, dir.FullName));
      foreach (var fileInfo in dir.EnumerateFiles("*", SearchOption.TopDirectoryOnly))
      {
        var file = AddFile(source: fileInfo.Name);
        dataset.AppendTo("hasPart", file);
      }
    }

    // Add files in the top level of `source`
    foreach (var f in dirInfo.EnumerateFiles("*", SearchOption.TopDirectoryOnly))
    {
      AddFile(source: f.Name);
    }

    Metadata.Write(source);
    Preview.Write(source);
  }

  /// <summary>
  /// Check the metadata of an RO-Crate.
  /// </summary>
  /// <param name="source">
  /// The path to the directory containing the RO-Crate to initialise an <c>ROCrate</c> from.
  /// </param>
  /// <exception cref="DirectoryNotFoundException">Can't find the source directory.</exception>
  /// <exception cref="FileNotFoundException">Can't find the metadata JSON file.</exception>
  /// <exception cref="JsonException">The metadata file is invalid JSON.</exception>
  /// <exception cref="InvalidDataException">The metadata format is not satisfied.</exception>
  private void CheckSourceMetadata(string source)
  {
    if (!Directory.Exists(source))
      throw new DirectoryNotFoundException($"{source} does not exist or is not a directory.");

    var metadataPath = Path.Combine(source, "ro-crate-metadata.json");
    if (!System.IO.File.Exists(metadataPath))
      throw new FileNotFoundException(
        $@"Metadata file not found in {source}. If you would like this to be an RO-Crate, use Convert to convert it."
      );

    var json = System.IO.File.ReadAllText(metadataPath);
    JsonNode? metadataJson;
    try
    {
      metadataJson = JsonNode.Parse(json);
      if (metadataJson is null) throw new JsonException("Metadata file contains no metadata");
    }
    catch (JsonException)
    {
      throw new JsonException("Metadata file contains no metadata");
    }

    var metadataObject = metadataJson.AsObject();

    if (!(metadataObject.ContainsKey("@context") || metadataObject.ContainsKey("@graph")))
      throw new InvalidDataException("Metadata is missing @context, @graph or both.");
  }
}
