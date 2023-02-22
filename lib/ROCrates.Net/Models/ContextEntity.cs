using System.Text.Json.Nodes;

namespace ROCrates.Models;

public class ContextEntity : Entity
{
  public ContextEntity(ROCrate crate, string? identifier = null, JsonObject? properties = null) : base(crate,
    identifier, properties)
  {
  }
}
