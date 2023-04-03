using System.Text.Json;
using System.Text.Json.Nodes;
using ROCrates.Converters;

namespace ROCrates.Models;

public class Person : ContextEntity
{
  public Person(ROCrate? crate = null, string? identifier = null, JsonObject? properties = null) : base(crate,
    identifier,
    properties)
  {
    DefaultType = "Person";
    Properties = _empty();
    if (properties is not null) _unpackProperties(properties);
  }

  public Person()
  {
    DefaultType = "Person";
    Properties = _empty();
  }

  /// <summary>
  /// Convert <see cref="Person"/> to JSON string.
  /// </summary>
  /// <returns>The <see cref="Person"/> as a JSON string.</returns>
  public override string Serialize()
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Converters = { new EntityConverter<Person>() }
    };
    var serialised = JsonSerializer.Serialize(this, options);
    return serialised;
  }

  /// <summary>
  /// Create a <see cref="Person"/> from JSON properties.
  /// </summary>
  /// <param name="entityJson">The JSON representing the <see cref="Person"/></param>
  /// <param name="roCrate">The RO-Crate for the <see cref="Person"/></param>
  /// <returns>The deserialised <see cref="Person"/></returns>
  public new static Person? Deserialize(string entityJson, ROCrate roCrate)
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Converters = { new EntityConverter<Person>() }
    };
    var deserialized = JsonSerializer.Deserialize<Person>(entityJson, options);
    if (deserialized is not null) deserialized.RoCrate = roCrate;
    return deserialized;
  }
}
