using LinkLiteManager.Data;
using Microsoft.EntityFrameworkCore;

namespace LinkLiteManager.Services;

public class SourceTypeService
{
  private readonly ApplicationDbContext _db;

  public SourceTypeService(ApplicationDbContext db)
  {
    _db = db;
  }

  public async Task<List<Models.SourceType>> List()
  {
    
    var list = await _db.SourceTypes
      .AsNoTracking()
      .ToListAsync();
    return list.ConvertAll<Models.SourceType>(x=> new(x.Id));
  }
}
