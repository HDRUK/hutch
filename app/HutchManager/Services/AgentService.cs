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
  
  public async Task<Models.AgentSummary> Create(Models.ManageAgent manageAgent)
  {
    Agent agent = new Agent()
    {
      Name = manageAgent.Name,
      ClientId = manageAgent.ClientId,
      ClientSecretHash = manageAgent.ClientSecret.Sha256() // Has the secret
    };
    
    await _db.Agents.AddAsync(agent);
    await _db.SaveChangesAsync();
    return new AgentSummary() // return agent summary
    {
      Name = agent.Name, ClientId = agent.ClientId
    };
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
  public async Task<AgentSummary> Set(int id, ManageAgent agent)
  {
    var entity = await _db.Agents
                   .SingleOrDefaultAsync(x => x.Id == id)
                 ?? throw new KeyNotFoundException($"No Agent with ID: {id}");
    entity.Name = agent.Name;
    entity.ClientId = agent.ClientId;
    if (agent.ClientSecret != "") 
    {
      entity.ClientSecretHash = agent.ClientSecret.Sha256(); // hash the secret
    }
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
  /// Generate a new client id and secret 
  /// </summary>
  /// <param name="isNew"></param>
  /// <returns></returns>
  /// <exception cref="KeyNotFoundException"></exception>
  public Task<ManageAgent> Generate (bool isNew)
  {
    if (!isNew) // check if request is for an existing Agent. Only send Client secret.
      return Task.FromResult(new ManageAgent() { ClientSecret = Crypto.GenerateId() });
    
    return Task.FromResult(new ManageAgent() {// if request is for a new Agent registration, send both
      ClientId = Crypto.GenerateId(),
      ClientSecret = Crypto.GenerateId()
    });
  }
}
