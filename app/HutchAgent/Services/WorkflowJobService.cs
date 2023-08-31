using HutchAgent.Data;
using HutchAgent.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HutchAgent.Services;

public class WorkflowJobService
{
  private readonly HutchAgentContext _db;

  public WorkflowJobService(HutchAgentContext db)
  {
    _db = db;
  }

  /// <summary>
  /// Create an entry in the database for the given <see cref="WorkflowJob"/>.
  /// </summary>
  /// <param name="jobId">The ID of the job to be added to the database.</param>
  /// <param name="workDir">The working directory of the job to be added to the database.</param>
  /// <returns>The ID of the created <see cref="WorkflowJob"/>.</returns>
  public async Task<string> Create(string jobId, string workDir)
  {
    var entity = new WorkflowJob()
    {
      Id = jobId,
      WorkingDirectory = workDir,
      StartTime = DateTimeOffset.UtcNow,
    };

    await _db.AddAsync(entity);
    await _db.SaveChangesAsync();
    return jobId;
  }

  /// <summary>
  /// List all <see cref="WorkflowJob"/>s in the database.
  /// </summary>
  /// <returns>The list of <see cref="Models.WorkflowJob"/>s in the database.</returns>
  public async Task<List<Models.WorkflowJob>> List()
  {
    return await _db.WorkflowJobs
      .AsNoTracking()
      .Select(entity => new Models.WorkflowJob
      {
        Id = entity.Id,
        WorkingDirectory = entity.WorkingDirectory,
        ExecutorRunId = entity.ExecutorRunId,
        ExitCode = entity.ExitCode,
        StartTime = entity.StartTime,
        EndTime = entity.EndTime
      }
      ).ToListAsync();
  }

  /// <summary>
  /// Get a <see cref="WorkflowJob"/> with the given ID from the database.
  /// </summary>
  /// <param name="jobId">The ID of the <see cref="WorkflowJob"/> to be retrieved from the database.</param>
  /// <returns><see cref="Models.WorkflowJob"/> with the requested ID.</returns>
  public async Task<WorkflowJob> Get(string jobId)
  {
    return await _db.WorkflowJobs.FindAsync(jobId)
      ?? throw new KeyNotFoundException();
  }

  /// <summary>
  /// Update a <see cref="WorkflowJob"/> in the database with the properties of the passed job.
  /// </summary>
  /// <param name="job"><see cref="Models.WorkflowJob"/> describing the intended state of the Job</param>
  /// <returns><see cref="Models.WorkflowJob"/> describing the resulting state of the Job</returns>
  /// <exception cref="KeyNotFoundException"></exception>
  public async Task<Models.WorkflowJob> Set(Models.WorkflowJob job)
  {
    var entity = await _db.WorkflowJobs.FindAsync(job.Id) ?? throw new KeyNotFoundException();

    _db.Entry(entity).CurrentValues.SetValues(job);
    await _db.SaveChangesAsync();

    return new()
    {
      Id = entity.Id,
      WorkingDirectory = entity.WorkingDirectory,
      ExecutorRunId = entity.ExecutorRunId,
      ExitCode = entity.ExitCode,
      StartTime = entity.StartTime,
      EndTime = entity.EndTime,
    };
  }

  /// <summary>
  /// Remove a <see cref="WorkflowJob"/> with the given ID from the database.
  /// </summary>
  /// <param name="jobId">The ID of the <see cref="WorkflowJob"/> to be removed from the database.</param>
  /// <returns></returns>
  public async Task Delete(string jobId)
  {
    var entry = await _db.WorkflowJobs.FindAsync(jobId);
    if (entry is not null)
    {
      _db.WorkflowJobs.Remove(entry);
      await _db.SaveChangesAsync();
    }
  }
}
