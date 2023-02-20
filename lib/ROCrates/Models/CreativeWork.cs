using System.Text.Json.Nodes;

namespace ROCrates.Models;

public class CreativeWork : Entity
{
  public CreativeWork(ROCrate crate, string? identifier, JsonObject? properties) : base(crate, identifier, properties)
  {
  }
}
