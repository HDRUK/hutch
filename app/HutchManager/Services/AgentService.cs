using HutchManager.Data;
using HutchManager.Data.Entities;
using HutchManager.Extensions;

namespace HutchManager.Services;

public class AgentService
{
  private readonly ApplicationDbContext _db;
  
  public AgentService(ApplicationDbContext db)
  {
    _db = db;
  }
  
  public async Task<Models.AgentModel> Create(Models.ManageAgent manageAgent)
  {
    Agent agent = new Agent()
    {
      Name = manageAgent.Name,
      ClientId = manageAgent.ClientId,
      ClientSecretHash = manageAgent.ClientSecretHash.Sha256()
    };
    
    await _db.Agents.AddAsync(agent);
    await _db.SaveChangesAsync();
    return new(agent);
  }
}
