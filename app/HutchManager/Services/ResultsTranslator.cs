using System.Text.Json;
using HutchManager.Dto;
using Schema.NET;

namespace HutchManager.Services;

public class ResultsTranslator
{
  
  public interface IResultsTranslator<T>
  {
    T TranslateRoCrates(ROCratesQueryResult input);
  }
  
  public class RoCratesQueryTranslator : IResultsTranslator<QueryResult>
  {

    public QueryResult TranslateRoCrates(ROCratesQueryResult job)
    {
      var rquestQueryResult = new QueryResult();
      foreach (var graph in job.Graphs)
      {
        var property = SchemaSerializer.DeserializeObject<PropertyValue>(graph.ToString());
        if (property == null) throw new InvalidDataException();
        switch (property.Name)
        {
          case "activity_source_id":
            int activitySourceId = int.Parse(property.Value!);
            rquestQueryResult.ActivitySourceId = activitySourceId;
            break;
          case "job_id":
            var jobId = property.Value;
            rquestQueryResult.JobId = jobId!; 
            break;
          case "count":
            int count = int.Parse(property.Value!);
            rquestQueryResult.Results.Count = count;
            break;
        }
      }
      return rquestQueryResult ;
    }
  }

  public class RoCratesToDistribution : IResultsTranslator<DistributionQueryTaskResult>
  {
    public DistributionQueryTaskResult TranslateRoCrates(ROCratesQueryResult job)
    {
      var rquestQueryResult = new DistributionQueryTaskResult();
      foreach (var graph in job.Graphs)
      {
        if (graph.TryGetProperty("@type", out var type))
        {
          switch (type.ToString())
          {
            case "PropertyValue":
              var property = SchemaSerializer.DeserializeObject<PropertyValue>(graph.ToString());
              if (property == null) throw new InvalidDataException();
              switch (property.Name)
              {
                case "activity_source_id":
                  int activitySourceId = int.Parse(property.Value!);
                  rquestQueryResult.ActivitySourceId = activitySourceId;
                  break;
                case "status":
                  rquestQueryResult.Status = property.Value!;
                  break;
                case "job_id":
                  rquestQueryResult.JobId = property.Value!;
                  break;
                case "count":
                  rquestQueryResult.QueryResult.Count = int.Parse(property.Value!);
                  break;
                case "datasetCount":
                  rquestQueryResult.QueryResult.DatasetsCount = int.Parse(property.Value!);
                  break;
              }
              break;
            case "ItemList":
              var itemList = SchemaSerializer.DeserializeObject<ItemList>(graph.ToString());
              if (itemList == null) throw new InvalidDataException();
              var fileObject = new FileObject();
              foreach (var item in itemList.ItemListElement)
              {
                var fileProperty = SchemaSerializer.DeserializeObject<PropertyValue>(item.ToString());
                if (fileProperty == null) throw new InvalidDataException();
                switch (fileProperty.Name)
                {
                  case "fileData":
                    fileObject.Data = fileProperty.Value.ToString()!;
                    break;
                  case "fileDescription":
                    fileObject.Description = fileProperty.Value;
                    break;
                  case "fileName":
                    fileObject.Name = fileProperty.Value!;
                    break;
                  case "fileReference":
                    fileObject.Reference = fileProperty.Value!;
                    break;
                  case "fileSensitive":
                    fileObject.Sensitive = (bool)fileProperty.Value!;
                    break;
                  case "fileSize":
                    fileObject.Size = (double)fileProperty.Value!;
                    break;
                  case "fileType":
                    fileObject.Type = fileProperty.Value!;
                    break;
                }
              }
              break;
          }
        }
      }
      return rquestQueryResult;
    }
  }
}
