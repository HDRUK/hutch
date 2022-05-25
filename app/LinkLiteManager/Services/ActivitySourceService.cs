using LinkLiteManager.Data;
using LinkLiteManager.Models;
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
      .ToListAsync();
    return list.ConvertAll<Models.ActivitySource>(x => new(x.Id,x.Host,x.Type,x.ResourceId));
  }
  public async Task<Models.ActivitySource> Create(Models.CreateActivitySource activitySource)
  {
    await _db.ActivitySources.AddAsync(activitySource);
    await _db.SaveChangesAsync();
  }
}
