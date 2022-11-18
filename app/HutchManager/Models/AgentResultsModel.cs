namespace HutchManager.Models;

public class AgentResultsModel
{
  public int Id { get; set; }

  public string Name { get; set; }
  
  public string ClientId { get; set; }
  
  public List<Data.Entities.DataSource> DataSources { get; set; }

  public AgentResultsModel(Data.Entities.Agent entity)
  {
    Id = entity.Id;
    Name = entity.Name;
    ClientId = entity.ClientId;
    DataSources = entity.DataSources;
  }
}
