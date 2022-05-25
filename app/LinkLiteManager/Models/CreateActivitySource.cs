using System.ComponentModel.DataAnnotations;

namespace LinkLiteManager.Models;

public record CreateActivitySource
(
  [Required]
  int Id,
  [Required]
  string Host, 
  [Required]
  string Type, 
  [Required]
  string ResourceId
);

