using System.Text.Json.Nodes;

namespace ROCrates.Models;

public class DataEntity : Entity
{
  public DataEntity(ROCrate crate, string? identifier = null, JsonObject? properties = null) : base(crate, identifier,
    properties)
  {
  }
}
