using LinkLiteManager.Data;
using Microsoft.EntityFrameworkCore;

namespace LinkLiteManager.Services;

public class ActivitySourceService
{
  private readonly ApplicationDbContext _db;

  public ActivitySourceService(ApplicationDbContext db)
  {
    _db = db;
  }
  
  public async Task<List<Models.ActivitySource>> List()
  {
    var list = await _db.ActivitySources
      .AsNoTracking()
      .Include(x=>x.Type)
      .ToListAsync();
    return list.ConvertAll<Models.ActivitySource>(x => new(x));
  }
  
  public async Task<Models.ActivitySource> Create(Models.CreateActivitySource activitySource)
  {
    var types = await _db.SourceTypes.ToListAsync();
    var entity = activitySource.ToEntity(types);
    
    await _db.ActivitySources.AddAsync(entity);
    await _db.SaveChangesAsync();
    return new(entity);
  }
  
  public async Task<Models.ActivitySource> Set(int id, Models.CreateActivitySource activitySource)
  {
    var entity = await _db.ActivitySources
      .Include(x => x.Type)
      .FirstOrDefaultAsync(x => x.Id == id);

    if (entity is null)
      throw new KeyNotFoundException(
        $"No ActivitySource with ID: {id}");
    entity.Host = activitySource.Host;
    entity.DisplayName = activitySource.DisplayName;
    entity.ResourceId = activitySource.ResourceId;
    await _db.SaveChangesAsync();
    return new (entity);
  }
  public async Task<Models.ActivitySource> Get(int activitySourceId)
  {
    var activitySource = await _db.ActivitySources
                     .AsNoTracking()
                     .Include(x=>x.Type)
                     .Where(x => x.Id == activitySourceId)
                     .SingleOrDefaultAsync()
                   ?? throw new KeyNotFoundException();
    return new(activitySource);
  }

  public async Task Delete(int activitySourceId)
  {
    var entity = await _db.ActivitySources
      .AsNoTracking()
      .Include(x=>x.Type)
      .FirstOrDefaultAsync(x => x.Id == activitySourceId);
    if (entity is null)
      throw new KeyNotFoundException(
        $"No ActivitySource with ID: {activitySourceId}");
    _db.ActivitySources.Remove(entity);
    await _db.SaveChangesAsync();

  }
}
