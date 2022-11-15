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

  /// <summary>
  /// Get all records in ModifierTypeModel
  /// </summary>
  /// <returns></returns>
  public async Task<List<Models.ModifierTypeModel>> GetTypes()
  {
    var list = await _db.ModifierTypes
      .AsNoTracking()
      .ToListAsync();
    return list.ConvertAll<Models.ModifierTypeModel>(x => new(x));
  }

  public async Task<ResultsModifierModel> Create(int activitySourceId, CreateResultsModifier resultsModifier)
  {
    var type = (await _db.ModifierTypes.ToListAsync()).FirstOrDefault(x => x.Id == resultsModifier.Type) ??
               throw new InvalidOperationException($"Type {resultsModifier.Type} is not a valid ModifierType");
    var activitySource =
      (await _db.ActivitySources.Include(x => x.Type).Include(x => x.TargetDataSource).ToListAsync()).FirstOrDefault(
        x => x.Id == activitySourceId) ??
      throw new KeyNotFoundException(
        $"Activity Source {resultsModifier.ActivitySourceId} is not a valid Activity Source");

    var limitCheck = (await _db.ResultsModifiers.ToListAsync())
      .Where(x => x.ActivitySource.Id == activitySourceId && x.Type.Id == type.Id);

    if (limitCheck.Count() >= type.Limit)
    {
      throw new BadHttpRequestException(
        $"Cannot create any more modifiers of type {type.Id} for the activity source {activitySource.DisplayName}");
    }

    var activitySourceModifiers = (await _db.ResultsModifiers.Include(x => x.ActivitySource).ToListAsync())
      .Where(x => x.ActivitySource.Id == activitySourceId)
      .ToList();

    var order = activitySourceModifiers.Count() + 1;//Set to be the last one
    var entity = resultsModifier.ToEntity(type, activitySource, order);
    await _db.ResultsModifiers.AddAsync(entity);
    await _db.SaveChangesAsync();
    return new(entity);
  }

  public async Task<ResultsModifierModel> Set(int id, UpdateResultsModifier resultsModifier)
  {
    var entity = await _db.ResultsModifiers
      .Include(x => x.Type)
      .FirstOrDefaultAsync(x => x.Id == id);

    if (entity is null)
      throw new KeyNotFoundException(
        $"No ResultsModifier with ID: {id}");

    var type = (await _db.ModifierTypes.ToListAsync()).FirstOrDefault(x => x.Id == resultsModifier.Type) ??
               throw new InvalidOperationException($"Type {resultsModifier.Type} is not a valid ModifierType");
    var activitySource =
      (await _db.ActivitySources.Include((x => x.TargetDataSource)).Include(x => x.Type).ToListAsync()).FirstOrDefault(x => x.Id == resultsModifier.ActivitySourceId) ??
      throw new InvalidOperationException(
        $"Activity Source {resultsModifier.ActivitySourceId} is not a valid Activity Source");

    entity.Type = type;
    entity.Parameters = resultsModifier.Parameters;
    entity.ActivitySource = activitySource;
    await _db.SaveChangesAsync();
    return new(entity);
  }

  /// <summary>
  /// Delete a ResultsModifier by ID
  /// </summary>
  /// <param name="resultsModifierId"></param>
  /// <exception cref="KeyNotFoundException"></exception>
  public async Task Delete(int resultsModifierId)
  {
    var entity = await _db.ResultsModifiers
      .AsNoTracking()
      .Include(x => x.Type)
      .Include(x => x.ActivitySource)
      .FirstOrDefaultAsync(x => x.Id == resultsModifierId);
    
    if (entity is null)
      throw new KeyNotFoundException(
        $"No Results Modifier with ID: {resultsModifierId}");
    _db.ResultsModifiers.Remove(entity);
    await UpdateOrderOnDelete(entity); // Update the order
    await _db.SaveChangesAsync();
  }

  /// <summary>
  /// Re-order all ResultsModifiers of an ActivitySource based on the newPosition
  /// </summary>
  /// <param name="id"></param>
  /// <param name="newPosition"></param>
  /// <returns></returns>
  /// <exception cref="KeyNotFoundException"></exception>
  /// <exception cref="InvalidOperationException"></exception>
  public async Task<ResultsModifierModel> SetOrder(int id, int newPosition)
  {
        var entity = await _db.ResultsModifiers
      .Include(x => x.Type)
      .Include(x => x.ActivitySource)
      .ThenInclude(x => x.TargetDataSource)
      .Include(x => x.ActivitySource)
      .ThenInclude(x => x.Type)
      .FirstOrDefaultAsync(x => x.Id == id) ?? throw new KeyNotFoundException($"No ResultsModifier with ID: {id}");

    var activitySourceModifiers = (await _db.ResultsModifiers
                                    .Include(x => x.Type)
                                    .Include(x => x.ActivitySource)
                                    .ToListAsync())
                                    .Where(x => x.ActivitySource.Id == entity.ActivitySource.Id)
                                    .ToList() ??
                                  throw new KeyNotFoundException($"Activity Source {entity.ActivitySource.Id} is not a valid Activity Source");

    if (newPosition <= 0) newPosition = 1; // Set to 1 if newPosition is 0 or negative

    if (newPosition > activitySourceModifiers.Count()) newPosition = activitySourceModifiers.Count(); // Set to last position if newPosition is higher than the count 

    if (newPosition == entity.Order) return new(entity); // Make no changes if newPosition is the same as current one

    activitySourceModifiers = activitySourceModifiers.OrderBy(o => o.Order).ToList(); // Match order to list index

    if (newPosition < entity.Order)
    {
      // moving upwards:
      // 1. Insert the updated ResultsModifier record later
      // 2. Delete the old record
      var i = activitySourceModifiers.IndexOf(entity);
      entity.Order = newPosition;
      activitySourceModifiers.Insert(newPosition - 1, entity);
      activitySourceModifiers.RemoveAt(i + 1);
    }
    else
    {
      // moving downwards:
      // 1. Delete the old record
      // 2. Insert the updated ResultsModifier at the new position
      activitySourceModifiers.Remove(entity);
      entity.Order = newPosition;
      activitySourceModifiers.Insert(newPosition - 1, entity);
    }
    
    // Change the Order for the rest of the resultsModifiers in the ActivitySource
    var index = 0;
    foreach (var resultsModifier in activitySourceModifiers)
    {
      resultsModifier.Order = index + 1;
      index++;
    }
    
    await _db.SaveChangesAsync();
    return new(entity);
  }
  
  /// <summary>
  /// Update the ResultsModifiers order based on the removed ResultsModifier item
  /// </summary>
  /// <param name="entity"></param>
  private async Task UpdateOrderOnDelete(ResultsModifier entity)
  {
    var activitySourceModifiers = (await _db.ResultsModifiers
                                    .Include(x => x.Type)
                                    .Include(x => x.ActivitySource)
                                    .ToListAsync())
                                  .Where(x => x.ActivitySource.Id == entity.ActivitySource.Id)
                                  .ToList() ??
                                  throw new KeyNotFoundException($"Activity Source {entity.ActivitySource.Id} is not a valid Activity Source");
    
    foreach (var resultsModifier in activitySourceModifiers)
    {
      if (resultsModifier.Order > entity.Order) // Only update the order that are below the deleted item
        resultsModifier.Order = resultsModifier.Order - 1; // Update the order by substracting their original order value by 1
    }
  }
}
