using System.ComponentModel.DataAnnotations.Schema;

namespace HutchManager.Data.Entities;

[Table("logs")]
public class Logs
{
  [Column("id")]
  public int Id { get; set; }
  [Column("exception")]
  public string? Exception { get; set; } = string.Empty;
  [Column("level")]
  public string Level { get; set; } = string.Empty;
  [Column("message")]
  public string Message { get; set; } = string.Empty;
  [Column("messagetemplate")]
  public string MessageTemplate { get; set; } = string.Empty;
  [Column("properties")]
  public string Properties { get; set; } = string.Empty;
  [Column("timestamp")]
  public DateTime TimeStamp { get; set; }
}
