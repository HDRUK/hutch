using System.Text.Json;
using System.Text.Json.Nodes;
using ROCrates.Converters;

namespace ROCrates.Models;

public class ComputerLanguage : ContextEntity
{
  public ComputerLanguage(ROCrate? crate = null, string? identifier = null, JsonObject? properties = null) : base(crate,
    identifier, properties)
  {
    DefaultType = "ComputerLanguage";
    Properties = _empty();
    if (properties is not null) _unpackProperties(properties);
  }

  public string? Name
  {
    get => GetProperty<string>("name");
    set => SetProperty("name", value);
  }

  public string? Url
  {
    get => GetProperty<string>("url");
    set => SetProperty("url", value);
  }

  public string? AlternativeName
  {
    get => GetProperty<string>("alternativeName");
    set => SetProperty("alternativeName", value);
  }

  public string? Version
  {
    get => GetProperty<string>("version");
    set => SetProperty("version", value);
  }

  public string? Identifier
  {
    get => GetProperty<string>("identifier");
    set => SetProperty("identifier", value);
  }

  /// <summary>
  /// Convert <see cref="ComputerLanguage"/> to JSON string.
  /// </summary>
  /// <returns>The <see cref="ComputerLanguage"/> as a JSON string.</returns>
  public override string Serialize()
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Converters = { new ComputerLanguageConverter() }
    };
    var serialised = JsonSerializer.Serialize(this, options);
    return serialised;
  }

  public new static ComputerLanguage? Deserialize(string entityJson, ROCrate roCrate)
  {
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      Converters = { new ComputerLanguageConverter() }
    };
    var deserialized = JsonSerializer.Deserialize<ComputerLanguage>(entityJson, options);
    if (deserialized is not null) deserialized.RoCrate = roCrate;
    return deserialized;
  }
}
