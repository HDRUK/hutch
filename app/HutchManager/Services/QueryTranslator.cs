using System.Data;
using HutchManager.Dto;


namespace HutchManager.Services;

public class QueryTranslator
{
  public interface IQueryTranslator<T>
  { 
    ROCratesQuery Translate(T job);
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
      
      //Add Group Operator
      graphs.Add(new ROCratesQuery.ROCratesGraph
      {
        Type = "PropertyValue",
        Name = "groupOperator",
        Value = job.Query.Combinator
      });

      //Add query type
      graphs.Add(new ROCratesQuery.ROCratesGraph
      {
        Type = "PropertyValue",
        Name = "query_type",
        Value = "RQuestAvailability"
      });
      
      foreach (var group in job.Query.Groups)
      {
        var graph = new ROCratesQuery.ROCratesGraph();
        var itemListElements = new List<ROCratesQuery.Item>();
        
        foreach (var rule in group.Rules)
        {
          graph.Type = "ItemList";
          graph.Name = "group";
          graph.NumberOfItems = group.Rules.Count;
          
          var itemListElement = new ROCratesQuery.Item();
          itemListElement.Type = "QuantitativeValue";
          
          
          if (rule.Type == "NUM")
          {
            //For "NUM" Rules
            //Split the Rule's VariableName at "="
            //Save the Rule's OMOP Concept ID in the Item's Value.
            itemListElement.Value = rule.VariableName.Split("=")[1];
            //Split the Rule's Value to get the minValue and maxValue
            var range = rule.Value.Split("..");
            itemListElement.MinValue = range[0];
            itemListElement.MaxValue = range[1];
          }
          else
          {
            //For "TEXT" Rules
            itemListElement.Value = rule.Value;
            itemListElement.AdditionalProperty = new ROCratesQuery.Property
            {
              Type = "PropertyValue",
              Name = "operator",
              Value = rule.Operand,
            };
          }
          //Add the Item to a List
          itemListElements.Add(itemListElement);
          //Save the list to the Graph's ItemListElement
          graph.ItemListElement = itemListElements;
        }
        //Create an Item for the Rule's Operator
        var ruleOperator = new ROCratesQuery.Item
        {
          Type = "PropertyValue",
          Name = "ruleOperator",
          Value = group.Combinator
        };
        graph.ItemListElement = graph.ItemListElement.Append(ruleOperator);
        graphs.Add(graph);
      }
      roCratesQuery.Graphs = graphs;
      return roCratesQuery;
    }
  }
  
  public class RquestDistributionQueryTranslator: IQueryTranslator<RquestDistributionQueryTask>
  {
    public ROCratesQuery Translate(RquestDistributionQueryTask job)
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
      
      // Add analysis code
      graphs.Add(new ROCratesQuery.ROCratesGraph
      {
        Type = "PropertyValue",
        Name = "code",
        Value = job.Code
      });
      
      // Add analysis type
      graphs.Add(new ROCratesQuery.ROCratesGraph
      {
        Type = "PropertyValue",
        Name = "analysis",
        Value = job.Analysis
      });

      //Add query type
      graphs.Add(new ROCratesQuery.ROCratesGraph
      {
        Type = "PropertyValue",
        Name = "query_type",
        Value = "RQuestDistribution"
      });
      
      // Add the graph to the query
      roCratesQuery.Graphs = graphs;
      
      return roCratesQuery;
    }
  }
}
