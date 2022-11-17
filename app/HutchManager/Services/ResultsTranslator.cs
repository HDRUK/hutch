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
    /// <summary>
    /// Translate the results of an availability query from RO-Crates format to RQuest.
    /// </summary>
    /// <param name="job"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException">Thrown when RO-Crate object cannot be deserialized.</exception>
    public QueryResult TranslateRoCrates(ROCratesQueryResult job)
    {
      var rquestQueryResult = new QueryResult();
      foreach (var graph in job.Graphs)
      {
        // Try to deserialize property value or throw exception
        var property = SchemaSerializer.DeserializeObject<PropertyValue>(graph.ToString());
        if (property == null) throw new InvalidDataException();
        // assign values to appropriate fields in result
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
    /// <summary>
    /// Translate the results of a distribution query from RO-Crates format to RQuest.
    /// </summary>
    /// <param name="job"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException">Thrown when RO-Crate object cannot be deserialized.</exception>
    public DistributionQueryTaskResult TranslateRoCrates(ROCratesQueryResult job)
    {
      var rquestQueryResult = new DistributionQueryTaskResult();
      foreach (var graph in job.Graphs)
      {
        // Try to get the `@type` property of the JSON object. 
        if (graph.TryGetProperty("@type", out var type))
        {
          // Determine the type of the JSON object.
          switch (type.ToString())
          {
            // If it is a PropertyValue, it represents non-file attributes of the query result.
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
            // If it is an ItemList, it represents a file object.
            case "ItemList":
              var itemList = SchemaSerializer.DeserializeObject<ItemList>(graph.ToString());
              if (itemList == null) throw new InvalidDataException();
              var fileObject = new FileObject();
              foreach (var item in itemList.ItemListElement)
              {
                if (item == null) continue;
                var fileProperty = SchemaSerializer.DeserializeObject<PropertyValue>(item.ToString()!);
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
        // No `@type`, cannot parse.
        else
        {
          throw new InvalidDataException();
        }
      }
      return rquestQueryResult;
    }
  }
}
