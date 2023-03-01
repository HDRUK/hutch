using System.Text.Json.Nodes;

namespace ROCrates.Models;

public class SoftwareApplication : ContextEntity
{
  public SoftwareApplication(ROCrate crate, string? identifier = null, JsonObject? properties = null) : base(crate,
    identifier, properties)
  {
    DefaultType = "SoftwareApplication";
    Properties = _empty();
    if (properties is not null) _unpackProperties(properties);
  }
}
