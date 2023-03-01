using System.Text.Json.Nodes;

namespace ROCrates.Models;

public class ContextEntity : Entity
{
  public ContextEntity(ROCrate crate, string? identifier = null, JsonObject? properties = null) : base(crate,
    identifier, properties)
  {
    Identifier = _formatIdentifier(Identifier);
    if (properties is not null) _unpackProperties(properties);
  }

  private protected sealed override string _formatIdentifier(string identifier)
  {
    if (Uri.IsWellFormedUriString(identifier, UriKind.RelativeOrAbsolute) || identifier.Contains('#'))
      return identifier;
    
    return "#" + identifier;
  }
}
