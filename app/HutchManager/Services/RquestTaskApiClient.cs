using Flurl;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using HutchManager.Data;
using HutchManager.Data.Entities;
using HutchManager.Dto;
using HutchManager.OptionsModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using JsonException = System.Text.Json.JsonException;
using JsonSerializer = System.Text.Json.JsonSerializer;


namespace HutchManager.Services
{
    public class RquestTaskApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<RquestTaskApiClient> _logger;
        private readonly RquestTaskApiOptions _apiOptions;
        private readonly ApplicationDbContext _db;
        public RquestTaskApiClient(
            HttpClient client,
            ILogger<RquestTaskApiClient> logger,
            IOptions<RquestTaskApiOptions> apiOptions,
            ApplicationDbContext db)
        {
          
            _client = client;
            _logger = logger;
            _apiOptions = apiOptions.Value;
            _db = db;
            
            string credentials = _apiOptions.Username + ":" + _apiOptions.Password;
            var authString = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
            
            _client.BaseAddress = new Uri(Url.Combine(_apiOptions.BaseUrl, "/"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authString);

  
        }

        /// <summary>
        /// Serialize a value to a JSON string, and provide HTTP StringContent
        /// for it with a media type of "application/json"
        /// </summary>
        /// <param name="value"></param>
        /// <returns>HTTP StringContent with the value serialized to JSON and a media type of "application/json"</returns>
        private StringContent AsHttpJsonString<T>(T value)
            => new StringContent(
                    JsonConvert.SerializeObject(value),
                    System.Text.Encoding.UTF8,
                    "application/json");

        /// <summary>
        /// Try and get a job for a collection
        /// </summary>
        /// <param name="activitySource"> ActivitySource</param>
        /// <returns>A Task DTO containing a Query to run, or null if none are waiting</returns>
        public async Task<RquestQueryTask?> FetchQuery(ActivitySource activitySource)
        {
          
          string requestUri = (Url.Combine(_apiOptions.FetchQueryEndpoint, "/", activitySource.ResourceId));
          var result = await _client.GetAsync(
              requestUri);

            if (result.IsSuccessStatusCode)
            {
                if (result.StatusCode == HttpStatusCode.NoContent)
                {
                    _logger.LogInformation(
                        "No Query Jobs waiting for {_resourceId}",
                        activitySource.ResourceId);
                    return null;
                }

                try
                {
                    var job = await result.Content.ReadFromJsonAsync<RquestQueryTask>();

                    // a null job is impossible because the necessary JSON payload
                    // to achieve it would fail deserialization
                    _logger.LogInformation($"Found Query with Id: {job!.JobId}");
                    //Set ActivitySource ID
                    job.ActivitySourceId = activitySource.Id;
                    return job;
                }
                catch (JsonException e)
                {
                    _logger.LogError(e, "Invalid Response Format from Fetch Query Endpoint");

                    var body = await result.Content.ReadAsStringAsync();
                    _logger.LogDebug("Invalid Response Body: {body}", body);

                    throw;
                }
            }
            else
            {
                var message = $"Fetch Query Endpoint Request failed: {result.StatusCode}";
                _logger.LogError(message);
                throw new ApplicationException(message);
            }
        }

        /// <summary>
        /// Post to the Results endpoint, and handle the response correctly
        /// </summary>
        /// <param name="activitySourceId">activitySourceId ID</param>
        /// <param name="jobId">Job ID</param>
        /// <param name="count">Optional Count for submitting results</param>
        public async Task ResultsEndpointPost(int activitySourceId, string jobId, QueryResultCount result)
        {
          var activitySource = await _db.ActivitySources
            .FirstOrDefaultAsync(x => x.Id == activitySourceId);
      
          if (activitySource is null)
            throw new KeyNotFoundException(
              $"No ActivitySource with ID: {activitySourceId}");
         
          string resourceId = activitySource.ResourceId.Remove(activitySource.ResourceId.Length - 2);
         
          string requestUri = (Url.Combine(_apiOptions.SubmitResultEndpoint, "/", jobId, "/", resourceId));
          _logger.LogInformation("Arguements {resourceId} {jobId}",resourceId,jobId);
          var payload = new RquestQueryTaskResult(resourceId, jobId, result.Count);
          var response = (await _client.PostAsync(
            requestUri,AsHttpJsonString(payload))).EnsureSuccessStatusCode();
          
         
          // however, even if 2xx we need to check the body for success status
          string body = string.Empty;
          try
          {
            body = await response.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<RquestResultResponse>(body);

            if (json?.Status != "OK")
            {
              var message = "Unsuccessful Response from Submit Results Endpoint";
              _logger.LogError(message);
              _logger.LogDebug("Response Body: {body}", body);

              throw new ApplicationException(message);
            }

            return;
          }
          catch (JsonException e)
          {
            _logger.LogError(e, "Invalid Response Format from Submit Results Endpoint");
            _logger.LogDebug("Invalid Response Body: {body}", body);

            throw;
          }
          
        }
    }
}
