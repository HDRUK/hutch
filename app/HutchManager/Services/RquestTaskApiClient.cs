using Flurl;
using Microsoft.Extensions.Options;

using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using HutchManager.Data.Entities;
using HutchManager.Dto;
using HutchManager.OptionsModels;


namespace HutchManager.Services
{
    public class RquestTaskApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<RquestTaskApiClient> _logger;
        private readonly RquestTaskApiOptions _apiOptions;

        public RquestTaskApiClient(
            HttpClient client,
            ILogger<RquestTaskApiClient> logger,
            IOptions<RquestTaskApiOptions> apiOptions)
        {
          
            _client = client;
            _logger = logger;
            _apiOptions = apiOptions.Value;
            
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
                    JsonSerializer.Serialize(value),
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
        /// Submit the result of a query
        /// </summary>
        /// <param name="jobId">ID of the query task</param>
        /// <param name="count">The result</param>
        public async Task SubmitQueryResult(string jobId, int count) => await ResultsEndpointPost(jobId, count);

        /// <summary>
        /// Cancel a query task
        /// </summary>
        /// <param name="jobId">ID of the query task</param>
        public async Task CancelQueryTask(string jobId) => await ResultsEndpointPost(jobId);

        /// <summary>
        /// Post to the Results endpoint, and handle the response correctly
        /// </summary>
        /// <param name="jobId">Job ID</param>
        /// <param name="count">Optional Count for submitting results</param>
        private async Task ResultsEndpointPost(string jobId, int? count = null)
        {
            
            var response = (await _client.PostAsync(
                    _apiOptions.SubmitResultEndpoint,
                    AsHttpJsonString(new RquestQueryTaskResult(jobId, count))))
                .EnsureSuccessStatusCode();
            
            // however, even if 2xx we need to check the body for sucess status
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
