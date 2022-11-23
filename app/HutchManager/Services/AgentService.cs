using HutchManager.Data;
using HutchManager.Models;
using Microsoft.EntityFrameworkCore;

namespace HutchManager.Services;

public class AgentService
{
  private readonly ApplicationDbContext _db;

  public AgentService(ApplicationDbContext db)
  {
    _db = db;
  }

  /// <summary>
  /// Get all Agents
  /// </summary>
  /// <returns></returns>
  public async Task<List<AgentDataSource>> List()
  {
    var list = await _db.Agents
      .AsNoTracking()
      .Include(x => x.DataSources)
      .Select(x=> new AgentDataSource()
      {
        Id = x.Id,
        Name = x.Name,
        ClientId = x.ClientId,
        DataSources = x.DataSources.Select(y=>y.Id).ToList(),
      })
      .ToListAsync();
    
    return list;
  }
  
  /// <summary>
  /// Get an Agent by ID
  /// </summary>
  /// <param name="agentId"></param>
  /// <returns></returns>
  /// <exception cref="KeyNotFoundException"></exception>
  public async Task<AgentDataSource> Get(int agentId)
  {
    var agent = await _db.Agents
                  .AsNoTracking()
                  .Include(x => x.DataSources)
                  .Where(x=> x.Id ==agentId)
                  .Select(x=> new AgentDataSource()
                  {
                    Id = x.Id,
                    Name = x.Name,
                    ClientId = x.ClientId,
                    DataSources = x.DataSources.Select(y=>y.Id).ToList(),
                  }).SingleOrDefaultAsync() 
                ?? throw new KeyNotFoundException();
    return agent;
  }
}
