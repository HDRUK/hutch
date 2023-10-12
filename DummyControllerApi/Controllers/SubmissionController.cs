using System.Text.Json;
using DummyControllerApi.Config;
using DummyControllerApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DummyControllerApi.Controllers;

// Disclaimer:
// Much of this controller is not how I would expect to conventionally build an ASP.NET Core API
// or a REST API at all for that matter (query string parameters, much?)
// However it is vital it matches the specification of the TRE Controller API for testing interactions.

[ApiController]
[Route("api/[controller]")]
public class SubmissionController : ControllerBase
{
  private readonly ILogger<SubmissionController> _logger;
  private readonly EgressBucketDetailsOptions _bucketOptions;

  public SubmissionController(
    ILogger<SubmissionController> logger,
    IOptions<EgressBucketDetailsOptions> bucketOptions)
  {
    _logger = logger;
    _bucketOptions = bucketOptions.Value;
  }

  [HttpPost("UpdateStatusForTre")]
  public async Task<IActionResult> UpdateStatusForTre(
    [FromQuery] string subId,
    [FromQuery] int statusType,
    [FromQuery] string description)
  {
    // Cursory format validation

    // TODO we actually don't know what unsuccessful reponses the real API returns under what conditions
    // but the validation here should at least help make sure Hutch's request behaviours are as expected

    if (string.IsNullOrWhiteSpace(subId)) return BadRequest("Expected a subId");

    if (statusType is < 30 or > 42)
      return BadRequest("Status Type was outside the expected enum range for Hutch");

    // documented return type is a `text/plain` encoded json object :S
    return Ok(JsonSerializer.Serialize(new UpdateStatusResponseModel()));
  }

  [HttpPost("FilesReadyForReview")]
  public async Task<IActionResult> FilesReadyForReview([FromQuery] string subId)
  {
    // TODO presumably we'll need to queue a delayed task that responds with approval
    // AFTER this endpoint gives a response
    // so that Hutch is ready for the /approval request
  }

  [HttpGet("GetOutputBucketInfo")]
  public IActionResult GetOutputBucketInfo([FromQuery] string subId)
  {
    // TODO we actually don't know what unsuccessful reponses the real API returns under what conditions
    // but the validation here should at least help make sure Hutch's request behaviours are as expected
    
    if(string.IsNullOrWhiteSpace(subId))
      return BadRequest("Expected a subId");
    
    // Unknown what encoding etc is expected here; just the content
    return Ok(new EgressBucketResponseModel
    {
      Host = _bucketOptions.Host,
      Bucket = _bucketOptions.Bucket
    });
  }
}
