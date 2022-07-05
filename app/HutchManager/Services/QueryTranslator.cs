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
      
      roCratesQuery.Context = "https://w3id.org/ro/crate/1.1/context";
      int length = rquestQuery.Groups.Count();
      for (int i=0;i<length;i++)
      {
        var ruleLength = rquestQuery.Groups[i].Rules.Count();
        for (int j = 0; j < ruleLength; j++)
        {
          var rule = rquestQuery.Groups[i].Rules[j];
          Console.WriteLine(rule.Value);

          var initialGraph = new ROCratesQuery.ROCratesGraph();
          initialGraph.Context = "https://schema.org";
          initialGraph.Type = "ItemList";
          initialGraph.Type = "group";
          initialGraph.NumberOfItems = ruleLength;
          
          var item = new ROCratesQuery.Item();
          item.Context = "https://schema.org";
          item.Type = rule.Type;
          item.Value = rule.Value;

          var properties = new ROCratesQuery.Property();
          
          properties.Context = "https://schema.org";
          properties.Type = rule.Type;
          properties.Name = "operator";
          properties.Value = rule.Operand;
          
          roCratesQuery.Graphs= initialGraph;
          roCratesQuery.Graphs[i].ItemListElements[j] = item;
          roCratesQuery.Graphs[i].ItemListElements[j].AdditionalProperties[j] = properties;
        }
      }
      return roCratesQuery;
    }
  }
}
