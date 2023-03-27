using System.Text.Json;
using System.Text.Json.Nodes;
using ROCrates.Converters;

namespace ROCrates.Models;

public class CreativeWork : Entity
{
  public CreativeWork(ROCrate? crate = null, string? identifier = null, JsonObject? properties = null) : base(crate,
    identifier,
    properties)
  {
  }

  /// <summary>
  /// Convert <see cref="CreativeWork"/> to JSON string.
  /// </summary>
  /// <returns>The <see cref="CreativeWork"/> as a JSON string.</returns>
  public override string Serialize()
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Converters = { new CreativeWorkConverter() }
    };
    var serialised = JsonSerializer.Serialize(this, options);
    return serialised;
  }
}
