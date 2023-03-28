using System.Text.Json;
using System.Text.Json.Nodes;
using ROCrates.Converters;

namespace ROCrates.Models;

public class ContextEntity : Entity
{
  public ContextEntity(ROCrate crate, string? identifier = null, JsonObject? properties = null) : base(crate,
    identifier, properties)
  {
    Id = _formatIdentifier(Id);
    if (properties is not null) _unpackProperties(properties);
  }

  private protected sealed override string _formatIdentifier(string identifier)
  {
    if (Uri.IsWellFormedUriString(identifier, UriKind.RelativeOrAbsolute) || identifier.Contains('#'))
      return identifier;

    return "#" + identifier;
  }

  /// <summary>
  /// Convert <see cref="ContextEntity"/> to JSON string.
  /// </summary>
  /// <returns>The <see cref="ContextEntity"/> as a JSON string.</returns>
  public override string Serialize()
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Converters = { new ContextEntityConverter() }
    };
    var serialised = JsonSerializer.Serialize(this, options);
    return serialised;
  }
}
