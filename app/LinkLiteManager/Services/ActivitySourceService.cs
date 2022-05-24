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
      .ToListAsync();

    return list.ConvertAll<Models.ActivitySource>(x => new(x.Id,x.Host,x.Type.Id,x.ResourceId));
  }
}
