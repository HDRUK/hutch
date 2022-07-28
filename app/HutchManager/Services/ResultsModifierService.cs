using HutchManager.Data;
using HutchManager.Data.Entities;
using HutchManager.Models;
using Microsoft.EntityFrameworkCore;

namespace HutchManager.Services;

public class ResultsModifierService
{
  private readonly ApplicationDbContext _db;
  public ResultsModifierService(ApplicationDbContext db)
  {
    _db = db;
  }

  public async Task<List<Models.ResultsModifierModel>> List()
  {
    var list = await _db.ResultsModifier
      .AsNoTracking()
      .Include(x => x.Type)
      .Include(x => x.ActivitySource)
        .ThenInclude(x => x.Type)
      .Include(x => x.ActivitySource)
        .ThenInclude(x => x.TargetDataSource)
      .ToListAsync();
    return list.ConvertAll<Models.ResultsModifierModel>(x => new(x));
  }

  public async Task<List<Models.ModifierTypeModel>> GetTypes()
  {
    var list = await _db.ModifierTypes
      .AsNoTracking()
      .ToListAsync();
    return list.ConvertAll<Models.ModifierTypeModel>(x => new(x));
  }

  public async Task<Models.ResultsModifierModel> Create(Models.CreateResultsModifier resultsModifier)
  {


    var type = (await _db.ModifierTypes.ToListAsync()).FirstOrDefault(x => x.Id == resultsModifier.Type) ??
       throw new InvalidOperationException($"Type {resultsModifier.Type} is not a valid ModifierType");
    var activitySource = (await _db.ActivitySources.ToListAsync()).FirstOrDefault(x => x.Id == resultsModifier.ActivitySourceId) ??
       throw new InvalidOperationException($"Activity Source {resultsModifier.ActivitySourceId} is not a valid Activity Source");
 
    var limitCheck = (await _db.ResultsModifier.ToListAsync())
      .Where(x => x.ActivitySource.Id == activitySource.Id && x.Type.Id == type.Id );

    if(limitCheck.Count() >= type.Limit)
    {
      throw new BadHttpRequestException($"Cannot create any more modifiers of type {type.Id} for the activity source {activitySource.DisplayName}");
    }

    var entity = resultsModifier.ToEntity(type,activitySource);
    await _db.ResultsModifier.AddAsync(entity);
    await _db.SaveChangesAsync();
    return new(entity);
  }

  public async Task<Models.ResultsModifierModel> Set(int id, Models.CreateResultsModifier resultsModifier)
  {
    var entity = await _db.ResultsModifier
      .Include(x => x.Type)
      .FirstOrDefaultAsync(x => x.Id == id);

    if (entity is null)
      throw new KeyNotFoundException(
        $"No ResultsModifier with ID: {id}");
    
    var type = (await _db.ModifierTypes.ToListAsync()).FirstOrDefault(x => x.Id == resultsModifier.Type) ??
       throw new InvalidOperationException($"Type {resultsModifier.Type} is not a valid ModifierType");
    var activitySource = (await _db.ActivitySources.ToListAsync()).FirstOrDefault(x => x.Id == resultsModifier.ActivitySourceId) ??
       throw new InvalidOperationException($"Activity Source {resultsModifier.ActivitySourceId} is not a valid Activity Source");
    entity.Order = resultsModifier.Order;
    entity.Type = type;
    entity.Parameters = resultsModifier.Parameters;
    entity.ActivitySource = activitySource;
    await _db.SaveChangesAsync();
    return new(entity);
  }
  public async Task<Models.ResultsModifierModel> Get(int resultsModifierId)
  {
    var resultsModifier = await _db.ResultsModifier
                     .AsNoTracking()
                     .Include(x => x.Type)
                     .Include(x=>x.ActivitySource.Type)
                     .Include(x=>x.ActivitySource.TargetDataSource)
                     .Where(x => x.Id == resultsModifierId)
                     .SingleOrDefaultAsync()
                   ?? throw new KeyNotFoundException();
    return new(resultsModifier);
  }
  
  public async Task<List<Models.ResultsModifierModel>> GetActivitySourceResultsModifier(int activitySourceId)
  {
    var resultsModifierList = await _db.ResultsModifier
                            .AsNoTracking()
                            .Include(x => x.Type)
                            .Include(x=>x.ActivitySource)
                            .Include(x=> x.ActivitySource.Type)
                            .Include(x => x.ActivitySource.TargetDataSource)
                            .Where(x => x.ActivitySource.Id == activitySourceId)
                            .ToListAsync();
    return resultsModifierList.ConvertAll<Models.ResultsModifierModel>(x => new(x));
  }

  public async Task Delete(int resultsModifierId)
  {
    var entity = await _db.ResultsModifier
      .AsNoTracking()
      .Include(x => x.Type)
      .FirstOrDefaultAsync(x => x.Id == resultsModifierId);

    if (entity is null)
      throw new KeyNotFoundException(
        $"No Results Modifier with ID: {resultsModifierId}");
    _db.ResultsModifier.Remove(entity);
    await _db.SaveChangesAsync();

  }

}





