using System.Text.Json;
using ROCrates.Models;

namespace ROCrates.Converters;

public class TestInstanceConverter : EntityConverter
{
  public override TestInstance? Read(ref Utf8JsonReader reader, Type typeToConvert,
    JsonSerializerOptions options)
  {
    var properties = _parseJson(ref reader);

    // Object is invalid if there is no @id or @type key
    if (Id is null || Type is null)
      throw new InvalidDataException("Either one of, or both @id and @type are not in the JSON.");

    var testInstance = new TestInstance(identifier: Id, properties: properties);
    return testInstance;
  }
}
