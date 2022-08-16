using HutchManager.Dto;


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
      var graphs = new ROCratesQuery().Graphs;
      //Add ActivitySourceID
      graphs.Add(new ROCratesQuery.ROCratesGraph
      {
        Type = "PropertyValue",
        Name = "activity_source_id",
        Value = job.ActivitySourceId.ToString()
      });
      
      // Add Job ID
      graphs.Add(new ROCratesQuery.ROCratesGraph
      {
        Type = "PropertyValue",
        Name = "job_id",
        Value = job.JobId
      });
      foreach (var group in job.Query.Groups)
      {

        var graph = new ROCratesQuery.ROCratesGraph()
        {
          Type = "ItemList",
          Name = "group",
          NumberOfItems = group.Rules.Count,
          ItemListElements = group.Rules.Select(rule =>
            new ROCratesQuery.Item()
            {
              Type = rule.Type,
              Value = rule.Value,
              AdditionalProperty = new ROCratesQuery.Property()
              {
                Type = rule.Type,
                Name = "operator",
                Value = rule.Operand,
              }
            }
          )
        };
        var ruleOperator = new ROCratesQuery.Item()
        {
          Type = "PropertyValue",
          Name = "ruleOperator",
          Value = group.Combinator
        };
        graph.ItemListElements = graph.ItemListElements.Append(ruleOperator);
        graphs.Add(graph);
      }
      roCratesQuery.Graphs = graphs;
      return roCratesQuery;
    }
  }
}
