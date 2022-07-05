using HutchManager.Data.Entities;
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
      var Graph = new ROCratesQuery().Graphs;
      Graph.Add(new ROCratesQuery.ROCratesGraph());
      //Add ActivitySourceID
      Graph[0].Context = "https://schema.org";
      Graph[0].Type = "PropertyValue";
      Graph[0].Name = "activity_source_id";
      Graph[0].Value = job.ActivitySourceId;
      
      // Add Job ID
      Graph[1].Context = "https://schema.org";
      Graph[1].Type = "PropertyValue";
      Graph[1].Name = "activity_source_id";
      Graph[1].Value = job.JobId;
      
      var item = new List<ROCratesQuery.Item>();
      
      roCratesQuery.Context = "https://w3id.org/ro/crate/1.1/context";
      int length = job.Query.Groups.Count()+2;
      //For each group in an rquestQuery create a Graph in roCratesQuery
      for (int i=2;i<length;i++)
      {
        var ruleLength = job.Query.Groups[i].Rules.Count();
        Graph.Add(new ROCratesQuery.ROCratesGraph());
        Graph[i].Context = "https://schema.org";
        Graph[i].Type = "ItemList";
        Graph[i].Type = "group";
        Graph[i].NumberOfItems = ruleLength;
       
      //For each rule in a query create an Item in a Graph
        for (int j = 0; j < ruleLength; j++)
        {
          var rule = job.Query.Groups[i].Rules[j];
          
          item.Add(new ROCratesQuery.Item());
          item[j].Context = "https://schema.org";
          item[j].Type = rule.Type;
          item[j].Value = rule.Value;
          
          var properties = new List<ROCratesQuery.Property>();
          properties.Add(new ROCratesQuery.Property());
          properties[j].Context = "https://schema.org";
          properties[j].Type = rule.Type;
          properties[j].Name = "operator";
          properties[j].Value = rule.Operand;
          
          item[j].AdditionalProperties = properties;
          Graph[i].ItemListElements = item;
          roCratesQuery.Graphs = Graph;
          
          
        }
      }
      return roCratesQuery;
    }
  }
}
