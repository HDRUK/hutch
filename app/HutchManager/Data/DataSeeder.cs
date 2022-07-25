using HutchManager.Constants;
using HutchManager.Data.Entities;

namespace HutchManager.Data;

public class DataSeeder
{
  private readonly ApplicationDbContext _db;

  public DataSeeder(ApplicationDbContext db)
  {
    _db = db;
  }

  public async Task SeedSourceTypes()
  {
    var types = new List<SourceType>
    {
      new() { Id = SourceTypes.RQuest }
    };
    
    var existingTypeIds = _db.SourceTypes
      .Select(x => x.Id)
      .ToList();

    foreach (var t in types)
    {
      if (!existingTypeIds.Contains(t.Id))
        await _db.SourceTypes.AddAsync(t);
    }

    await _db.SaveChangesAsync();
  }
  
  public async Task SeedModifierTypes()
  {
    var types = new List<ModifierType>
    {
      new() { Id = ModifierTypes.LowNumberSuppression }
    };
    
    var existingTypeIds = _db.ModifierTypes
      .Select(x => x.Id)
      .ToList();

    foreach (var t in types)
    {
      if (!existingTypeIds.Contains(t.Id))
        await _db.ModifierTypes.AddAsync(t);
    }

    await _db.SaveChangesAsync();
  }
}
