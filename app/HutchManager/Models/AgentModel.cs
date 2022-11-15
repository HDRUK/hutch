namespace HutchManager.Models;

public class AgentModel
{
  public int Id { get; set; }

  public string Name { get; set; }
  
  public string ClientId { get; set; }

  public string ClientSecretHash { get; set; }
  
  public AgentModel(Data.Entities.Agent entity)
    {
      Id = entity.Id;
      Name = entity.Name;
      ClientId = entity.ClientId;
      ClientSecretHash = entity.ClientSecretHash;
    }
}


