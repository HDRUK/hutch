using System.IO.Compression;
using HutchAgent.Config;
using HutchAgent.Constants;
using HutchAgent.Models;
using HutchAgent.Results;
using Microsoft.Extensions.Options;
using ROCrates.Exceptions;

namespace HutchAgent.Services;

/// <summary>
/// This service contains shared functionality for accepting submission of Job Crates 
/// </summary>
public class RequestCrateService
{
  private readonly CrateService _crates;
  private readonly ILogger<RequestCrateService> _logger;

  public RequestCrateService(
    ILogger<RequestCrateService> logger,
    CrateService crates)
  {
    _logger = logger;
    _crates = crates;
  }

  /// <summary>
  /// Perform cursory validation of the Crate submission;
  /// enough to know whether we accept it in principle to try and Execute later.
  /// </summary>
  /// <param name="cratePath">Path to an RO-Crate root (i.e. with metadata)</param>
  /// <returns>A <see cref="BasicResult"/> indicating the outcome of the validation.</returns>
  public BasicResult IsValidToAccept(string cratePath)
  {
    var result = new BasicResult();

    // TODO: BagIt checksum validation? or do this during execution?

    // Validate that it's an RO-Crate at all, by trying to Initialise it
    try
    {
      _crates.InitialiseCrate(cratePath);
    }
    catch (Exception e) when (e is CrateReadException || e is MetadataException)
    {
      result.Errors.Add("The provided file is not an RO-Crate.");
    }

    // TODO: 5 safes crate profile validation? or do this during execution?

    result.Success = true;
    return result;
  }

  /// <summary>
  /// Unzips a zipped 5 Safes RO-Crate inside a job's working directory.
  /// </summary>
  /// <param name="job">A model describing the job the crate is for.</param>
  /// <param name="crate">A stream of the crate's bytes.</param>
  /// <returns>The path where the crate was unpacked.</returns>
  public string Unpack(WorkflowJob job, Stream crate)
  {
    var targetPath = job.WorkingDirectory.JobBagItRoot();

    using var archive = new ZipArchive(crate);

    Directory.CreateDirectory(targetPath);
    archive.ExtractToDirectory(targetPath, overwriteFiles: true);

    _logger.LogInformation("Crate extracted at {TargetPath}", targetPath);

    return targetPath;
  }
}
