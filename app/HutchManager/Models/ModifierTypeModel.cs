namespace HutchManager.Models;

public class ModifierTypeModel
{
  public string Id { get; set; }
  public int Limit { get; set; }

  public ModifierTypeModel(Data.Entities.ModifierType entity)
  {
    Id = entity.Id;
    Limit = entity.Limit;
  }
}
