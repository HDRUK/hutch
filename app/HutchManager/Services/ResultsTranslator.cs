using HutchManager.Dto;

namespace HutchManager.Services;

public class ResultsTranslator
{
  
  public interface IResultsTranslator<T>
  {
    T TranslateRoCrates(ROCratesQueryResult input);
  }
  
  public class RoCratesQueryTranslator : IResultsTranslator<QueryResult>
  {

    public  QueryResult TranslateRoCrates(ROCratesQueryResult job)
    {
      var rquestQueryResult = new QueryResult();
      foreach (var o in job.Graphs)
      {
        var graph = (PropertyValue)o;
        switch (graph.Name)
        {
          case "activity_source_id":
            int activitySourceId = Int32.Parse(graph.Value);
            rquestQueryResult.ActivitySourceId = activitySourceId;
            break;
          case "job_id":
            var jobId = graph.Value;
            rquestQueryResult.JobId = jobId;
            break;
          case "count":
            int count = Int32.Parse(graph.Value);
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
        var g = (PropertyValue)graph;
        switch (graph.Name)
        {
          case "activity_source_id":
            int activitySourceId = Int32.Parse(g.Value);
            rquestQueryResult.ActivitySourceId = activitySourceId;
            break;
          case "status":
            rquestQueryResult.Status = g.Value;
            break;
          case "job_id":
            rquestQueryResult.JobId = g.Value;
            break;
          case "files":
            var fileList = (ItemList<ItemList<PropertyValue>>)graph;
            // Iterate over files
            foreach (var f in fileList.ItemListElement)
            {
              var fileObject = new FileObject();
              // Iterate over file properties
              foreach (var prop in f.ItemListElement)
              {
                switch (prop.Name)
                {
                  case "fileData":
                    fileObject.Data = prop.Value;
                    break;
                  case "fileDescription":
                    fileObject.Description = prop.Value;
                    break;
                  case "fileName":
                    fileObject.Name = prop.Value;
                    break;
                  case "fileReference":
                    fileObject.Reference = prop.Value;
                    break;
                  case "fileSensitive":
                    fileObject.Sensitive = prop.Value == "True";
                    break;
                  case "fileSize":
                    fileObject.Size = float.Parse(prop.Value);
                    break;
                }
              }
              // Add file to list
              rquestQueryResult.QueryResult.Files.Add(fileObject);
            }
            break;
          case "count":
            rquestQueryResult.QueryResult.Count = int.Parse(g.Value);
            break;
          case "datasetCount":
            rquestQueryResult.QueryResult.DatasetsCount = int.Parse(g.Value);
            break;
        }

      }
      return rquestQueryResult;
    }
  }
}
