namespace HutchManager.Models;

public class ModifierTypeModel
{
  public string Id { get; set; } = string.Empty;
  public string Limit { get; set; } = string.Empty;

  public ModifierTypeModel(ModifierTypeModel entity)
  {
    Id = entity.Id;
    Limit = entity.Limit;
  }

  public ModifierTypeModel(Data.Entities.ModifierType entity)
  {
    Id = entity.Id;
    Limit = entity.Limit;
  }

  public ModifierTypeModel()
  {
  }
}
