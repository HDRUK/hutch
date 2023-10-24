using System.Text.Json.Serialization;

namespace HutchAgent.Config;

public class CratePublishingOptions
{
  /// <summary>
  /// Optional Publisher information to include when publishing Results Crates
  /// </summary>
  public PublisherOptions? Publisher { get; set; }

  /// <summary>
  /// Optional license information to include when publishing Results Crates
  /// </summary>
  public LicenseOptions? License { get; set; }
}

public class PublisherOptions
{
  /// <summary>
  /// An identifier for the publisher, typically a URL e.g. "https://tre72.example.com/"
  /// </summary>
  public string Id { get; set; } = string.Empty;
}

public class LicenseOptions
{
  /// <summary>
  /// The @id of the license entity to add to the outputs.
  /// </summary>
  public string Uri { get; set; } = string.Empty;

  /// <summary>
  /// This attribute contains any other properties for the license entity.
  /// </summary>
  public LicenseProperties? Properties { get; set; }
}

public class LicenseProperties
{
  [JsonPropertyName("name")] public string? Name { get; set; }

  [JsonPropertyName("identifier")] public string? Identifier { get; set; }
}
