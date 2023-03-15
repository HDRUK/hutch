using System.Text.Json;
using System.Text.Json.Serialization;
using ROCrates.Models;

namespace ROCrates.Serializers;

public class EntitySerializer : JsonConverter<Entity>
{
  public override Entity? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    throw new NotImplementedException();
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
