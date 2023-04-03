using System.Text.Json;
using System.Text.Json.Nodes;
using ROCrates.Converters;

namespace ROCrates.Models;

public class ContextEntity : Entity
{
  public ContextEntity(ROCrate? crate = null, string? identifier = null, JsonObject? properties = null) : base(crate,
    identifier, properties)
  {
    Id = _formatIdentifier(Id);
    if (properties is not null) _unpackProperties(properties);
  }

  public ContextEntity()
  {
    Id = _formatIdentifier(Id);
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
      Converters = { new EntityConverter<ContextEntity>() }
    };
    var serialised = JsonSerializer.Serialize(this, options);
    return serialised;
  }

  /// <summary>
  /// Create a <see cref="ContextEntity"/> from JSON properties.
  /// </summary>
  /// <param name="entityJson">The JSON representing the <see cref="ContextEntity"/></param>
  /// <param name="roCrate">The RO-Crate for the <see cref="ContextEntity"/></param>
  /// <returns>The deserialised <see cref="ContextEntity"/></returns>
  public new static ContextEntity? Deserialize(string entityJson, ROCrate roCrate)
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Converters = { new EntityConverter<ContextEntity>() }
    };
    var deserialized = JsonSerializer.Deserialize<ContextEntity>(entityJson, options);
    if (deserialized is not null) deserialized.RoCrate = roCrate;
    return deserialized;
  }
}
