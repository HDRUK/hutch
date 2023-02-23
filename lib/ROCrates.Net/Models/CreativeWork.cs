using System.Text.Json.Nodes;

namespace ROCrates.Models;

public class CreativeWork : Entity
{
  public CreativeWork(ROCrate crate, string? identifier = null, JsonObject? properties = null) : base(crate, identifier,
    properties)
  {
  }
}
