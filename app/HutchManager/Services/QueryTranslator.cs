using HutchManager.Dto;

namespace HutchManager.Services;

public class QueryTranslator
{
  public interface IQueryTranslator<T>
  { 
    ROCratesQuery Translate(T input);
  }
  public class RquestQueryTranslator: IQueryTranslator<RquestQuery>{
    public ROCratesQuery Translate(RquestQuery rquestQuery)
    {
      
      ROCratesQuery roCratesQuery = new();
      var Graph = new ROCratesQuery().Graphs;
      var item = new List<ROCratesQuery.Item>();
      
      roCratesQuery.Context = "https://w3id.org/ro/crate/1.1/context";
      int length = rquestQuery.Groups.Count();
      //For each group in an rquestQuery create a Graph in roCratesQuery
      for (int i=0;i<length;i++)
      {
        var ruleLength = rquestQuery.Groups[i].Rules.Count();
        Graph.Add(new ROCratesQuery.ROCratesGraph());
        Graph[i].Context = "https://schema.org";
        Graph[i].Type = "ItemList";
        Graph[i].Type = "group";
        Graph[i].NumberOfItems = ruleLength;
       
      //For each rule in a query create an Item in a Graph
        for (int j = 0; j < ruleLength; j++)
        {
          var rule = rquestQuery.Groups[i].Rules[j];
          
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
