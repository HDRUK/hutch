using System.Text.Json.Nodes;

namespace ROCrates.Models;

public class ContextEntity : Entity
{
  public ContextEntity(ROCrate crate, string? identifier, JsonObject? properties) : base(crate, identifier, properties)
  {
  }
}
