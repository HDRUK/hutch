using System.Text.Json.Serialization;

namespace ROCrates;

public class TestingExtraTerms
{
  [JsonPropertyName("TestSuite")] public string TestSuite = "https://w3id.org/ro/terms/test#TestSuite";

  [JsonPropertyName("TestInstance")] public string TestInstance = "https://w3id.org/ro/terms/test#TestInstance";

  [JsonPropertyName("TestService")] public string TestService = "https://w3id.org/ro/terms/test#TestService";

  [JsonPropertyName("TestDefinition")] public string TestDefinition = "https://w3id.org/ro/terms/test#TestDefinition";

  [JsonPropertyName("PlanemoEngine")] public string PlanemoEngine = "https://w3id.org/ro/terms/test#PlanemoEngine";
  
  [JsonPropertyName("JenkinsService")] public string JenkinsService = "https://w3id.org/ro/terms/test#JenkinsService";
  
  [JsonPropertyName("TravisService")] public string TravisService = "https://w3id.org/ro/terms/test#TravisService";
  
  [JsonPropertyName("GithubService")] public string GithubService = "https://w3id.org/ro/terms/test#GithubService";
  
  [JsonPropertyName("instance")] public string Instance = "https://w3id.org/ro/terms/test#instance";
  
  [JsonPropertyName("runsOn")] public string RunsOn = "https://w3id.org/ro/terms/test#runsOn";
  
  [JsonPropertyName("resource")] public string Resource = "https://w3id.org/ro/terms/test#resource";
  
  [JsonPropertyName("definition")] public string Definition = "https://w3id.org/ro/terms/test#definition";
  
  [JsonPropertyName("engineVersion")] public string EngineVersion = "https://w3id.org/ro/terms/test#engineVersion";
}
