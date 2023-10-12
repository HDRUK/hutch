using Microsoft.AspNetCore.Mvc;

namespace DummyControllerApi.Controllers;

[ApiController]
public class SubmissionController : ControllerBase
{
  private readonly ILogger<SubmissionController> _logger;

  public SubmissionController(ILogger<SubmissionController> logger)
  {
    _logger = logger;
  }
}
