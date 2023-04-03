using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using ROCrates.Models;

namespace ROCrates.Converters;

public class EntityConverter<T> : JsonConverter<T> where T : Entity, new()
{
  private protected string? Id = string.Empty;
  private protected string? Type = string.Empty;

  public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    var properties = _parseJson(ref reader);

    // Object is invalid if there is no @id or @type key
    if (Id is null || Type is null)
      throw new InvalidDataException("Either one of, or both @id and @type are not in the JSON.");

    var entity = new T
    {
      Id = Id,
      Properties = properties
    };
    return entity;
  }

  private protected JsonObject _parseJson(ref Utf8JsonReader reader)
  {
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
              Id = reader.GetString();
              break;
            case "@type":
              Type = reader.GetString();
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
        case JsonTokenType.StartObject:
          var part = _getPart(ref reader);
          var serialisedPart = JsonSerializer.SerializeToNode(part);
          if (currentKey is not null && serialisedPart is not null) properties.Add(currentKey, serialisedPart);
          break;
        case JsonTokenType.EndObject:
          break;
        case JsonTokenType.Number:
          if (reader.TryGetInt32(out var valueAsInt) && currentKey is not null)
          {
            properties.Add(currentKey, valueAsInt);
          }
          else if (reader.TryGetDouble(out var valueAsDouble) && currentKey is not null)
          {
            properties.Add(currentKey, valueAsDouble);
          }

          break;
        case JsonTokenType.StartArray:
          var partList = _getPartList(ref reader);
          var serialisedList = JsonSerializer.SerializeToNode(partList);
          if (currentKey is not null && serialisedList is not null) properties.Add(currentKey, serialisedList);
          break;
      }
    }

    return properties;
  }

  public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
  {
    writer.WriteStartObject();
    foreach (var prop in value.Properties)
    {
      writer.WritePropertyName(prop.Key);
      prop.Value?.WriteTo(writer, options);
    }

    writer.WriteEndObject();
  }

  private List<Part>? _getPartList(ref Utf8JsonReader reader)
  {
    var partList = new List<Part>();
    while (reader.Read())
    {
      switch (reader.TokenType)
      {
        case JsonTokenType.StartObject:
        {
          var part = _getPart(ref reader);
          if (part is not null) partList.Add(part);
          break;
        }
        case JsonTokenType.EndArray:
          return partList.Count > 0 ? partList : null;
      }
    }

    return null;
  }

  private Part? _getPart(ref Utf8JsonReader reader)
  {
    string? currentKey = null;
    var id = "";
    while (reader.Read())
    {
      switch (reader.TokenType)
      {
        // Safely ignore these cases
        case JsonTokenType.StartObject:
          break;
        case JsonTokenType.EndObject:
          return id is not null ? new Part { Id = id } : null;
        case JsonTokenType.Comment:
          break;
        // Get property name
        case JsonTokenType.PropertyName:
          currentKey = reader.GetString();
          if (currentKey != "@id") throw new InvalidDataException("Illegal property in part.");
          break;
        // Get the part's ID or throw if invalid type or key found
        case JsonTokenType.String:
          if (currentKey is not null && currentKey == "@id") id = reader.GetString();
          break;
      }
    }

    return null;
  }
}
