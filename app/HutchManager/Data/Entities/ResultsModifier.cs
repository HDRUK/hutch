using HutchManager.Models;
using System.Text.Json;

namespace HutchManager.Data.Entities;

public class ResultsModifier
{
  public int Id { get; set; }
  public int Order { get; set; }
  public ActivitySourceModel ActivitySource { get; set; } = new();
  public ModifierTypeModel Type { get; set; } = new();

  public JsonDocument? Parameters { get; set; }



}
