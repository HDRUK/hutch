using HutchManager.Extensions;

namespace HutchManager.Models;

public class ManageAgent
{
  public int Id { get; set; }

  public string Name { get; set; } = string.Empty;

  public string ClientId { get; set; } = string.Empty;

  public string ClientSecretHash { get; set; } = string.Empty;

  public List<Data.Entities.DataSource> DataSources { get; set; } = new();
  
   public Data.Entities.Agent ToEntity()
     => new()
     {
       Id = Id,
       Name = Name,
       ClientId = ClientId,
       ClientSecretHash = ClientSecretHash.Sha256(),
       DataSources = DataSources
     };
}
