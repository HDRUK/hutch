using HutchManager.Data;
using HutchManager.Data.Entities;
using HutchManager.Extensions;
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

  /// <summary>
  /// Get all Agents
  /// </summary>
  /// <returns></returns>
  public async Task<List<AgentSummary>> List()
  {
    var list = await _db.Agents
      .AsNoTracking()
      .Include(x => x.DataSources)
      .Select(x=> new AgentSummary()
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
  public async Task<AgentSummary> Get(int agentId)
  {
    var agent = await _db.Agents
                  .AsNoTracking()
                  .Include(x => x.DataSources)
                  .Where(x => x.Id == agentId)
                  .Select(x => new AgentSummary()
                  {
                    Id = x.Id,
                    Name = x.Name,
                    ClientId = x.ClientId,
                    DataSources = x.DataSources.Select(y => y.Id).ToList(),
                  }).SingleOrDefaultAsync()
                ?? throw new KeyNotFoundException();
    return agent;
  }

  /// <summary>
  /// Delete an Agent by ID
  /// </summary>
  /// <param name="agentId"></param>
  /// <exception cref="KeyNotFoundException"></exception>
  public async Task Delete(int agentId)
  {
    var entity = await _db.Agents
      .AsNoTracking()
      .Include(x => x.DataSources)
      .FirstOrDefaultAsync(x => x.Id == agentId);
    if (entity is null)
      throw new KeyNotFoundException(
        $"No Agent with ID: {agentId}");
    _db.Agents.Remove(entity);
    await _db.SaveChangesAsync();
  }


}
