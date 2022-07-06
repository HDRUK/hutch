using HutchManager.Dto;
using Microsoft.AspNetCore.Authentication;

namespace HutchManager.Services;

public class QueryTranslator
{
  public interface IQueryTranslator<T>
  { 
    ROCratesQuery Translate(T input);
  }
  public class RquestQueryTranslator: IQueryTranslator<RquestQueryTask>{
    public ROCratesQuery Translate(RquestQueryTask job)
    {
      
      ROCratesQuery roCratesQuery = new();
      roCratesQuery.Context = "https://w3id.org/ro/crate/1.1/context";

      var graphs = new ROCratesQuery().Graphs;
      //Add ActivitySourceID
      graphs.Add(new ROCratesQuery.ROCratesGraph
      {
        Context = "https://schema.org",
        Type = "PropertyValue",
        Name = "activity_source_id",
        Value = job.ActivitySourceId.ToString()
      });
      
      // Add Job ID
      graphs.Add(new ROCratesQuery.ROCratesGraph
      {
        Context = "https://schema.org",
        Type = "PropertyValue",
        Name = "activity_source_id",
        Value = job.JobId
      });

      var item = new List<ROCratesQuery.Item>();
      foreach (var group in job.Query.Groups)
      {

        var graph = new ROCratesQuery.ROCratesGraph()
        {
          Context = "https://schema.org",
          Type = "ItemList",
          Name = "group",
          NumberOfItems = group.Rules.Count,
          ItemListElements = group.Rules.Select(rule =>
            new ROCratesQuery.Item()
            {
              Context = "https://schema.org",
              Type = rule.Type,
              Value = rule.Value,
              AdditionalProperties = new List<ROCratesQuery.Property>()
              {
                new ROCratesQuery.Property()
                {
                  Context = "https://schema.org",
                  Type = rule.Type,
                  Name = "operator",
                  Value = rule.Operand,
                }
              }
            }
          )
        };
        graphs.Add(graph);
      }

      roCratesQuery.Graphs = graphs;
      return roCratesQuery;
    }
  }
}
