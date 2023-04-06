using System.Text.Json;
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
  private Entity? _mainEntity = null;

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

      else if (entityType == typeof(Metadata))
      {
        Metadata = entity as Metadata;
      }

      else if (entityType.IsSubclassOf(typeof(DataEntity)))
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
}
