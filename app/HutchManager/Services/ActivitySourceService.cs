using HutchManager.Data;
using Microsoft.EntityFrameworkCore;

namespace HutchManager.Services;

public class ActivitySourceService
{
  private readonly ApplicationDbContext _db;

  public ActivitySourceService(ApplicationDbContext db)
  {
    _db = db;
  }

  public async Task<List<Models.ActivitySourceModel>> List()
  {
    var list = await _db.ActivitySources
      .AsNoTracking()
      .Include(x => x.Type)
      .Include(x => x.TargetDataSource)
      .ToListAsync();
    return list.ConvertAll<Models.ActivitySourceModel>(x => new(x));
  }

  public async Task<Models.ActivitySourceModel> Create(Models.CreateActivitySource activitySource)
  {
    var types = await _db.SourceTypes.ToListAsync();
    var dataSources = await _db.DataSources.ToListAsync();
    var entity = activitySource.ToEntity(types, dataSources);

    await _db.ActivitySources.AddAsync(entity);
    await _db.SaveChangesAsync();
    return new(entity);
  }

  public async Task<Models.ActivitySourceModel> Set(int id, Models.CreateActivitySource activitySource)
  {
    var entity = await _db.ActivitySources
      .Include(x => x.Type)
      .Include(x => x.TargetDataSource)
      .FirstOrDefaultAsync(x => x.Id == id);

    if (entity is null)
      throw new KeyNotFoundException(
        $"No ActivitySource with ID: {id}");
    entity.Host = activitySource.Host;
    entity.DisplayName = activitySource.DisplayName;
    entity.ResourceId = activitySource.ResourceId;
    await _db.SaveChangesAsync();
    return new(entity);
  }

  public async Task<Models.ActivitySourceModel> Get(int activitySourceId)
  {
    var activitySource = await _db.ActivitySources
                           .AsNoTracking()
                           .Include(x => x.Type)
                           .Include(x => x.TargetDataSource)
                           .SingleOrDefaultAsync(x => x.Id == activitySourceId)
                         ?? throw new KeyNotFoundException();
    return new(activitySource);
  }

  public async Task Delete(int activitySourceId)
  {
    var entity = await _db.ActivitySources
      .AsNoTracking()
      .Include(x => x.Type)
      .FirstOrDefaultAsync(x => x.Id == activitySourceId);
    if (entity is null)
      throw new KeyNotFoundException(
        $"No ActivitySource with ID: {activitySourceId}");
    _db.ActivitySources.Remove(entity);
    await _db.SaveChangesAsync();
  }
}
