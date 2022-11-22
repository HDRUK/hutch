using HutchManager.Data;

namespace HutchManager.Services;

public class AgentService
{
  private readonly ApplicationDbContext _db;
  
  public AgentService(ApplicationDbContext db)
  {
    _db = db;
  }
  
  public async Task<Models.AgentModel> Create(Models.ManageAgent agent)
  {
    var entity = agent.ToEntity();
    
    await _db.Agents.AddAsync(entity);
    await _db.SaveChangesAsync();
    return new(entity);
  }
}
