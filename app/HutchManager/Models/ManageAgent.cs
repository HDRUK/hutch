using HutchManager.Extensions;

namespace HutchManager.Models;

public class ManageAgent
{
  public string Name { get; set; } = string.Empty;

  public string ClientId { get; set; } = string.Empty;

  public string ClientSecretHash { get; set; } = string.Empty;
  
}
