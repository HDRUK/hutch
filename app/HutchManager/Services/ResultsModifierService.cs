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

    var entity = resultsModifier.ToEntity(type);
    

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
    entity.Order = resultsModifier.Order;
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





