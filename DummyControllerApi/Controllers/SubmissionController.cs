using Microsoft.AspNetCore.Mvc;

namespace DummyControllerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubmissionController : ControllerBase
{
  private readonly ILogger<SubmissionController> _logger;

  public SubmissionController(ILogger<SubmissionController> logger)
  {
    _logger = logger;
  }

  [HttpPost("UpdateStatusForTre")]
  public async Task<IActionResult> UpdateStatusForTre()
  {
    
  }
  
  [HttpPost("FilesReadyForReview")]
  public async Task<IActionResult> FilesReadyForReview([FromQuery] string subId)
  {
    // TODO presumably we'll need to queue a delayed task that responds with approval
    // AFTER this endpoint gives a response
    // so that Hutch is ready for the /approval request
  }
  
  [HttpGet("GetOutputBucketInfo")]
  public async Task<IActionResult> GetOutputBucketInfo([FromQuery] string subId)
  {
    // TODO where will we get the details? just the default configured store i guess
  }
}
