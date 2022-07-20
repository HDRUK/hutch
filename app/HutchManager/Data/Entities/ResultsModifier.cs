using System.ComponentModel.DataAnnotations;
using HutchManager.Models;
using System.Text.Json;

namespace HutchManager.Data.Entities;

public class ResultsModifier
{
  public int Id { get; set; }
  public int Order { get; set; }
  
  [Required]
  public ActivitySource ActivitySource { get; set; } = null!;
  
  [Required]
  public ModifierType Type { get; set; } = null!;

  public JsonDocument? Parameters { get; set; }



}
