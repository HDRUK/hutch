using System.Text.Json;
using DummyControllerApi.Config;
using DummyControllerApi.Models;
using DummyControllerApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;

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
  private readonly InMemoryApprovalQueue _approvalQueue;
  private readonly EgressBucketDetailsOptions _bucketOptions;
  private readonly WebHookService _webHookService;

  public SubmissionController(
    ILogger<SubmissionController> logger,
    IOptions<EgressBucketDetailsOptions> bucketOptions,
    InMemoryApprovalQueue approvalQueue, WebHookService webHookService)
  {
    _logger = logger;
    _approvalQueue = approvalQueue;
    _webHookService = webHookService;
    _bucketOptions = bucketOptions.Value;
  }

  [HttpPost("UpdateStatusForTre")]
  [SwaggerResponse(200, "Status update recorded", typeof(UpdateStatusResponseModel))]
  public IActionResult UpdateStatusForTre(
    [FromQuery] string subId,
    [FromQuery] int statusType,
    [FromQuery] string? description)
  {
    // Cursory format validation

    // TODO we actually don't know what unsuccessful reponses the real API returns under what conditions
    // but the validation here should at least help make sure Hutch's request behaviours are as expected

    if (statusType is < 30 or > 42)
      return BadRequest($"{nameof(statusType)} was outside the expected enum range for Hutch");

    _logger.LogInformation(
      "Submission [{SubId}] Status update: {StatusId} ({Description})",
      subId,
      statusType,
      description);

    return Ok(new UpdateStatusResponseModel());
  }

  [HttpPost("FilesReadyForReview")]
  public IActionResult FilesReadyForReview([FromBody] FilesReadyRequestModel model)
  {
    // TODO we actually don't know what unsuccessful reponses the real API returns under what conditions
    // but the validation here should at least help make sure Hutch's request behaviours are as expected

    if (!model.Files.Any())
      return BadRequest($"Expected at least one file in {nameof(model.Files)}");


    // We need to queue a delayed task that responds with approval
    // AFTER this endpoint gives a response
    // so that Hutch is ready for the /approval request
    _approvalQueue.Enqueue(model.SubId); // TODO could persist the file list later for funzies but rn Hutch doesn't care

    _logger.LogInformation("Submission [{SubId}] Files Queued for Approval", model.SubId);

    return Ok(); // Unsure of this response body and code; TODO confirm with swagger
  }

  [HttpGet("GetOutputBucketInfo")]
  [SwaggerResponse(200, "The requested bucket details", typeof(EgressBucketResponseModel))]
  public IActionResult GetOutputBucketInfo([FromQuery] string subId)
  {
    var details = new EgressBucketResponseModel
    {
      SubId = subId,
      Host = _bucketOptions.Host,
      Bucket = _bucketOptions.Bucket,
      Path = _bucketOptions.Path
    };

    _logger.LogInformation(
      "Submission [{SubId}] Egress Bucket Requested: {Details}",
      subId,
      JsonSerializer.Serialize(details));

    // Unknown what encoding etc is expected here; just the content - TODO confirm with Swagger
    return Ok(details);
  }

  [HttpPost("finalOutcome")]
  public async Task<IActionResult> FinalOutcome([FromBody] FinalOutcomeRequestModel model)
  {
    _logger.LogInformation(
      "Submission [{SubId}] FinalOutcome file submitted: {FileObjectId}",
      model.SubId,
      model.File);

    await _webHookService.SendFinalOutcome(model);
    // Unsure of this response body and code; TODO confirm with swagger
    return Ok();
  }
}
