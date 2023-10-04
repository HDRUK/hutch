using System.ComponentModel.DataAnnotations;

namespace HutchAgent.Models;

public class DatabaseConnectionDetails
{
  /// <summary>
  /// Database Server Hostname
  /// </summary>
  [Required]
  public string Hostname { get; set; } = string.Empty;
  
  /// <summary>
  /// Database Server Port. Defaults to PostgreSQL Default (5432)
  /// </summary>
  public int Port { get; set; } = 5432;
  
  /// <summary>
  /// Name of the Database to connect to
  /// </summary>
  [Required]
  public string Database { get; set; } = string.Empty;
  
  /// <summary>
  /// Username with access to the database
  /// </summary>
  [Required]
  public string Username { get; set; } = string.Empty;
  
  /// <summary>
  /// Password with access to the database
  /// </summary>
  [Required]
  public string Password { get; set; } = string.Empty;
}
