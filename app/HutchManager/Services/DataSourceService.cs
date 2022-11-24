using HutchManager.Data;
using HutchManager.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HutchManager.Services;

public class DataSourceService
{
  private readonly ApplicationDbContext _db;
  public DataSourceService(ApplicationDbContext db)
  {
    _db = db;
  }

  public async Task<List<Models.DataSource>> List()
  {
    var list = await _db.DataSources
      .AsNoTracking()
      .ToListAsync();
    return list.ConvertAll<Models.DataSource>(x => new(x));
  }

  public async Task<Models.DataSource> CreateOrUpdate(Models.DataSource dataSource, List<int> agents)
  {
    var entity = await _db.DataSources.Include(x=>x.Agents)
      .FirstOrDefaultAsync(x => x.Id == dataSource.Id);
    
    //Get Agent from id
    var agentIds = _db.Agents.Where(x => agents.Contains(x.Id)).ToList();
    
    if (entity is null)
    {
      // create new Datasource with LastCheckin set as now
      entity = new DataSource()
      {
        Id = dataSource.Id,
        LastCheckin = DateTimeOffset.UtcNow,
        Agents = agentIds
      };
      await _db.DataSources.AddAsync(entity);
    }
    else
    {
      // update Datasource LastCheckin to now
      entity.LastCheckin = DateTimeOffset.UtcNow;
      
      // check if new agents are checking in
      var uniqueAgents = agentIds.Except(entity.Agents).ToList();
      if (uniqueAgents.Any())
      {
        // add new agents to DataSource Agents
        entity.Agents.AddRange(uniqueAgents);
      }
    }
    await _db.SaveChangesAsync();
    return new(entity);
  }

  public async Task Delete(string dataSourceId)
  {
    var entity = await _db.DataSources
      .AsNoTracking()
      .FirstOrDefaultAsync(x => x.Id == dataSourceId);
    if (entity is null)
      throw new KeyNotFoundException(
        $"No DataSource with ID: {dataSourceId}");
    _db.DataSources.Remove(entity);
    await _db.SaveChangesAsync();

  }

}
