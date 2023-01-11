using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HutchManager.Dto;

public class AvailabilityQuery
{
  [JsonPropertyName("owner")]
  public string Owner { get; set; } = string.Empty;

  [JsonPropertyName("cohort")] 
  public Cohort Cohort { get; set; } = new();
  
  /// <summary>
  /// The collection ID to run the query against.
  /// </summary>
  [JsonPropertyName("collection")]
  public string Collection { get; set; } = String.Empty;
  
  [JsonPropertyName("protocol_version")]
  public string ProtocolVersion { get; set; } = String.Empty;
  
  [JsonPropertyName("char_salt")]
  public string CharSalt { get; set; } = String.Empty;
  
  [JsonPropertyName("uuid")]
  public string Uuid { get; set; } = String.Empty;
}

public class Cohort
{
  [JsonPropertyName("groups_oper")]
  public string Combinator { get; set; } = string.Empty;

  [JsonPropertyName("groups")]
  public List<Group> Groups { get; set; } = new ();
}

public class Group
{
  [JsonPropertyName("rules_oper")]
  public string Combinator { get; set; } = string.Empty;

  [JsonPropertyName("rules")]
  public List<Rule> Rules { get; set; } = new ();
}

public class Rule
{
  [JsonPropertyName("type")]
  public string Type { get; set; } = string.Empty;

  [JsonPropertyName("varname")]
  public string VariableName { get; set; } = string.Empty;

  /// <summary>
  /// Note that this isn't really an operand on the value
  /// it's pure inclusion or exclusion criteria: `=` or `!=`
  /// </summary>
  [JsonPropertyName("oper")]
  public string Operand { get; set; } = string.Empty;

  [JsonPropertyName("value")]
  public string Value { get; set; } = string.Empty;
        
  [JsonPropertyName("time")]
  public string Time { get; set; } = string.Empty;

  [JsonPropertyName("ext")]
  public string ExternalAttribute { get; set; } = string.Empty;

  /// <summary>
  /// Reserved for Future Use with some types (e.g. NUMERIC)
  /// 
  /// </summary>
  [JsonPropertyName("unit")]
  public string Unit { get; set; } = string.Empty;

  /// <summary>
  /// TEXT type only; the string to be uased for matching
  /// in a SQL `LIKE` expression (not, confusingly, a RegEx)
  /// </summary>
  [JsonPropertyName("regex")]
  public string RegEx { get; set; } = string.Empty;
}
