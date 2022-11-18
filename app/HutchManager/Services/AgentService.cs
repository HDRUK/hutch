using HutchManager.Data;
using HutchManager.Data.Entities;
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
  public async Task<List<Models.AgentResultsModel>> List()
  {
    var list = await _db.Agents
      .AsNoTracking()
      .Include(x => x.DataSources)
      .ToListAsync();
    
    return list.ConvertAll<Models.AgentResultsModel>(x => new(x));
  }
  
  /// <summary>
  /// Get an Agent by ID
  /// </summary>
  /// <param name="agentId"></param>
  /// <returns></returns>
  /// <exception cref="KeyNotFoundException"></exception>
  public async Task<Models.AgentResultsModel> Get(int agentId)
  {
    var agent = await _db.Agents
                  .AsNoTracking()
                  .Include(x => x.DataSources)
                  .SingleOrDefaultAsync(x => x.Id == agentId)
                ?? throw new KeyNotFoundException();
    return new(agent);
  }
}
