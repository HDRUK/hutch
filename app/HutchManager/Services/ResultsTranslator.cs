using HutchManager.Data;
using HutchManager.Dto;

namespace HutchManager.Services;

public class ResultsTranslator
{
  
  public interface IResultsTranslator<T>
  {
    QueryResult TranslateRoCrates(T input);
  }
  
  public class RoCratesQueryTranslator : IResultsTranslator<ROCratesQueryResult>
  {

    public  QueryResult TranslateRoCrates(ROCratesQueryResult job)
    {
      var rquestQueryResult = new QueryResult();
      foreach (var graph in job.Graphs)
      {
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
}
