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
      .ToListAsync();
    return list.ConvertAll<Models.ResultsModifierModel>(x => new(x));
  }

  public async Task<Models.ResultsModifierModel> Create(Models.CreateResultsModifier resultsModifier)
  {
    // Todo: Get this type from the database?
    /*
     var type = await _db.ModifierType.ToListAsync().FirstOrDefault(x => x.Id == resultsModifier.Type) ??
        throw new InvalidOperationException($"Type {resultsModifier.Type} is not a valid ModifierType")
     */
    var type = new ModifierTypeModel { Id = resultsModifier.Type };

    var entity = new ResultsModifier
    {
      Order = resultsModifier.Order,
      ActivitySource = resultsModifier.ActivitySource,
      Type = type,
      Parameters = resultsModifier.Parameters
    };

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
    // Todo: Get this type from the database?
    /*
     var type = await _db.ModifierType.ToListAsync().FirstOrDefault(x => x.Id == resultsModifier.Type) ??
        throw new InvalidOperationException($"Type {resultsModifier.Type} is not a valid ModifierType")
     */
    var type = new ModifierTypeModel { Id = resultsModifier.Type };
    entity.Order = resultsModifier.Order;
    entity.ActivitySource = resultsModifier.ActivitySource;
    entity.Type = type;
    entity.Parameters = resultsModifier.Parameters;
    await _db.SaveChangesAsync();
    return new(entity);
  }
  public async Task<Models.ResultsModifierModel> Get(int resultsModifierId)
  {
    var resultsModifier = await _db.ResultsModifier
                     .AsNoTracking()
                     .Include(x => x.Type)
                     .Where(x => x.Id == resultsModifierId)
                     .SingleOrDefaultAsync()
                   ?? throw new KeyNotFoundException();
    return new(resultsModifier);
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





