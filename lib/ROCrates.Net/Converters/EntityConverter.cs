using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using ROCrates.Models;

namespace ROCrates.Converters;

public class EntityConverter : JsonConverter<Entity>
{
  public override Entity? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? id = null;
    string? type = null;
    var properties = new JsonObject();
    string? currentKey = null;
    while (reader.Read())
    {
      switch (reader.TokenType)
      {
        case JsonTokenType.PropertyName:
          currentKey = reader.GetString();
          break;
        case JsonTokenType.String:
          if (currentKey is null) break;
          switch (currentKey)
          {
            case "@id":
              id = reader.GetString();
              break;
            case "@type":
              type = reader.GetString();
              break;
          }

          properties.Add(currentKey, reader.GetString());
          break;
        case JsonTokenType.True:
          if (currentKey is not null) properties.Add(currentKey, true);
          break;
        case JsonTokenType.False:
          if (currentKey is not null) properties.Add(currentKey, false);
          break;
        case JsonTokenType.Null:
          if (currentKey is not null) properties.Add(currentKey, null);
          break;
      }
    }

    // Object is invalid if there is no @id or @type key
    if (id is null || type is null)
      throw new InvalidDataException("Either one of, or both @id and @type are not in the JSON.");

    var entity = new Entity(identifier: id, properties: properties);
    return entity;
  }

  public override void Write(Utf8JsonWriter writer, Entity value, JsonSerializerOptions options)
  {
    writer.WriteStartObject();
    foreach (var prop in value.Properties)
    {
      writer.WritePropertyName(prop.Key);
      prop.Value?.WriteTo(writer, options);
    }

    writer.WriteEndObject();
  }
}
