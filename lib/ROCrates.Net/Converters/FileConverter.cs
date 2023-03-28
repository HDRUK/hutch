using System.Text.Json;
using File = ROCrates.Models.File;

namespace ROCrates.Converters;

public class FileConverter : EntityConverter
{
  public override File? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    var properties = _parseJson(ref reader);

    // Object is invalid if there is no @id or @type key
    if (Id is null || Type is null)
      throw new InvalidDataException("Either one of, or both @id and @type are not in the JSON.");

    var file = new File(identifier: Id, properties: properties);
    return file;
  }
}
