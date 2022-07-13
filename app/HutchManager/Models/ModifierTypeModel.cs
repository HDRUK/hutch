namespace HutchManager.Models;

public class ModifierTypeModel
{
  public string Id { get; set; } = string.Empty;
  public int Limit { get; set; }

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
