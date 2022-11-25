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
  /// Modify an Agent by id
  /// </summary>
  /// <param name="id"></param>
  /// <param name="agent"></param>
  /// <returns></returns>
  /// <exception cref="KeyNotFoundException"></exception>
  public async Task<AgentModel> Set(int id, ManageAgent agent)
  {
    var entity = await _db.Agents
                   .SingleOrDefaultAsync(x => x.Id == id)
                 ?? throw new KeyNotFoundException($"No Agent with ID: {id}");
    entity.Name = agent.Name;
    entity.ClientId = agent.ClientId;
    if (agent.ClientSecretHash != "")
    {
      entity.ClientSecretHash = agent.ClientSecretHash.Sha256();
    }
    await _db.SaveChangesAsync();
    return new (entity);
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
  /// Generate a new client id and secret 
  /// </summary>
  /// <param name="isNew"></param>
  /// <returns></returns>
  /// <exception cref="KeyNotFoundException"></exception>
  public async Task<ManageAgent> Generate (bool isNew)
  {
    // Generate a unique Client Id
    var clientId = string.Empty;
    var isClientIdUnique = false;
    while (!isClientIdUnique)
    {
      var uniqueClientId = Crypto.GenerateId();
      var existingClientIds = await _db.Agents.AnyAsync(x => x.ClientId == uniqueClientId);
      if (existingClientIds) continue;
      clientId = uniqueClientId;
      isClientIdUnique = true;
    }
    
    if (!isNew) // check if request is for an existing Agent. Only send Client secret.
      return new ManageAgent() { ClientSecret = Crypto.GenerateId() };
    
    return new ManageAgent() {// if request is for a new Agent registration, send both
      ClientId = clientId,
      ClientSecret = Crypto.GenerateId()
    };
  }


}
