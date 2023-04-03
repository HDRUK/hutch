using System.Text.Json;
using System.Text.Json.Nodes;
using ROCrates.Converters;

namespace ROCrates.Models;

public class SoftwareApplication : ContextEntity
{
  public SoftwareApplication(ROCrate? crate = null, string? identifier = null, JsonObject? properties = null) : base(
    crate,
    identifier, properties)
  {
    DefaultType = "SoftwareApplication";
    Properties = _empty();
    if (properties is not null) _unpackProperties(properties);
  }

  public SoftwareApplication()
  {
    DefaultType = "SoftwareApplication";
    Properties = _empty();
  }

  /// <summary>
  /// Convert <see cref="SoftwareApplication"/> to JSON string.
  /// </summary>
  /// <returns>The <see cref="SoftwareApplication"/> as a JSON string.</returns>
  public override string Serialize()
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Converters = { new EntityConverter<SoftwareApplication>() }
    };
    var serialised = JsonSerializer.Serialize(this, options);
    return serialised;
  }

  /// <summary>
  /// Create a <see cref="SoftwareApplication"/> from JSON properties.
  /// </summary>
  /// <param name="entityJson">The JSON representing the <see cref="SoftwareApplication"/></param>
  /// <param name="roCrate">The RO-Crate for the <see cref="SoftwareApplication"/></param>
  /// <returns>The deserialised <see cref="SoftwareApplication"/></returns>
  public new static SoftwareApplication? Deserialize(string entityJson, ROCrate roCrate)
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Converters = { new EntityConverter<SoftwareApplication>() }
    };
    var deserialized = JsonSerializer.Deserialize<SoftwareApplication>(entityJson, options);
    if (deserialized is not null) deserialized.RoCrate = roCrate;
    return deserialized;
  }
}
