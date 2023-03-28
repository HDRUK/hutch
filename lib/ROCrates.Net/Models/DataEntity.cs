using System.Text.Json;
using System.Text.Json.Nodes;
using ROCrates.Converters;

namespace ROCrates.Models;

public class DataEntity : Entity
{
  public DataEntity(ROCrate? crate = null, string? identifier = null, JsonObject? properties = null) : base(crate,
    identifier, properties)
  {
  }

  public virtual void Write(string basePath)
  {
    throw new NotImplementedException();
  }

  /// <summary>
  /// Convert <see cref="DataEntity"/> to JSON string.
  /// </summary>
  /// <returns>The <see cref="DataEntity"/> as a JSON string.</returns>
  public override string Serialize()
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Converters = { new DataEntityConverter() }
    };
    var serialised = JsonSerializer.Serialize(this, options);
    return serialised;
  }

  public new static DataEntity? Deserialize(string entityJson, ROCrate roCrate)
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Converters = { new DataEntityConverter() }
    };
    var deserialized = JsonSerializer.Deserialize<DataEntity>(entityJson, options);
    if (deserialized is not null) deserialized.RoCrate = roCrate;
    return deserialized;
  }
}
