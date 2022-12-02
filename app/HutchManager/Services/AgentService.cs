using HutchManager.Data;
using HutchManager.Data.Entities;
using HutchManager.Extensions;
using HutchManager.Helpers;
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
  
  public async Task<ManageAgent> Create(ManageAgent manageAgent)
  {
    var clientSecret = Crypto.GenerateId();
    Agent agent = new Agent()
    {
      Name = manageAgent.Name,
      ClientId = Crypto.GenerateId(),
      ClientSecretHash = clientSecret.Sha256() // Hash the secret
    };
    await _db.Agents.AddAsync(agent);
    await _db.SaveChangesAsync();
    return new ManageAgent() // return newly created agent
    {
      Id = agent.Id, Name = agent.Name, ClientId = agent.ClientId, ClientSecret = clientSecret
    };
  }

  /// <summary>
  /// Get all Agents
  /// </summary>
  /// <returns></returns>
  public async Task<List<ManageAgent>> List()
  {
    var list = await _db.Agents
      .AsNoTracking()
      .Include(x => x.DataSources)
      .Select(x=> new ManageAgent()
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
  public async Task<ManageAgent> Get(int agentId)
  {
    var agent = await _db.Agents
                  .AsNoTracking()
                  .Include(x => x.DataSources)
                  .Where(x => x.Id == agentId)
                  .Select(x => new ManageAgent()
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
  /// Modify an Agent by id
  /// </summary>
  /// <param name="id"></param>
  /// <param name="agent"></param>
  /// <returns></returns>
  /// <exception cref="KeyNotFoundException"></exception>
  public async Task<ManageAgent> Set(int id, ManageAgent agent)
  {
    var entity = await _db.Agents
                   .SingleOrDefaultAsync(x => x.Id == id)
                 ?? throw new KeyNotFoundException($"No Agent with ID: {id}");
    entity.Name = agent.Name;
    await _db.SaveChangesAsync();
    return await Get(id); // return a summary of the updated Agent
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

  /// <summary>
  /// Generate a new client secret and update the existing one
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  /// <exception cref="KeyNotFoundException"></exception>
  public async Task<ManageAgent> GenerateNewSecret (int id)
  {
    var clientSecret = Crypto.GenerateId();
    var entity = await _db.Agents
                   .SingleOrDefaultAsync(x => x.Id == id)
                 ?? throw new KeyNotFoundException($"No Agent with ID: {id}");
    entity.ClientSecretHash = Crypto.GenerateId().Sha256();
    await _db.SaveChangesAsync();
    return new ManageAgent() // return newly created agent
    {
      Id = id, Name = entity.Name, ClientId = entity.ClientId, ClientSecret = clientSecret
    }; // return a summary of the updated Agent
  }
}
