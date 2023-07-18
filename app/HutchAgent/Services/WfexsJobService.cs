using HutchAgent.Data;
using HutchAgent.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HutchAgent.Services;

public class WfexsJobService
{
  private readonly HutchAgentContext _db;

  public WfexsJobService(HutchAgentContext db)
  {
    _db = db;
  }

  /// <summary>
  /// Create an entry in the database for the given <see cref="WfexsJob"/>.
  /// </summary>
  /// <param name="job">The <see cref="WfexsJob"/> to be added to the database.</param>
  /// <returns>The created <see cref="WfexsJob"/></returns>
  public async Task<WfexsJob> Create(WfexsJob job)
  {
    var jobToAdd = new WfexsJob()
    {
      UnpackedPath = job.UnpackedPath,
      RunFinished = job.RunFinished,
      WfexsRunId = job.WfexsRunId
    };
    await _db.AddAsync(jobToAdd);
    await _db.SaveChangesAsync();
    return new WfexsJob()
    {
      Id = jobToAdd.Id, RunFinished = jobToAdd.RunFinished, UnpackedPath = jobToAdd.UnpackedPath,
      WfexsRunId = jobToAdd.WfexsRunId
    };
  }

  /// <summary>
  /// List all <see cref="WfexsJob"/>s in the database.
  /// </summary>
  /// <returns>The list of <see cref="WfexsJob"/>s in the database.</returns>
  public async Task<List<WfexsJob>> List()
  {
    var list = await _db.WfexsJobs
      .AsNoTracking()
      .Select(x => new WfexsJob
        {
          Id = x.Id,
          UnpackedPath = x.UnpackedPath,
          WfexsRunId = x.WfexsRunId,
          RunFinished = x.RunFinished,
          Pid = x.Pid
        }
      ).ToListAsync();
    return list;
  }

  public async Task<List<WfexsJob>> ListFinishedJobs()
  {
    return await _db.WfexsJobs.AsNoTracking().Where(x => x.RunFinished).ToListAsync();
  }

  /// <summary>
  /// Get a <see cref="WfexsJob"/> with the given ID from the database.
  /// </summary>
  /// <param name="jobId">The ID of the <see cref="WfexsJob"/> to be retrieved from the database.</param>
  /// <returns></returns>
  public async Task<WfexsJob> Get(int jobId)
  {
    var job = await _db.WfexsJobs.FindAsync(jobId);
    return job ?? throw new KeyNotFoundException();
  }

  /// <summary>
  /// Update a <see cref="WfexsJob"/> in the database with the properties of the passed job.
  /// </summary>
  /// <param name="job"></param>
  /// <returns></returns>
  /// <exception cref="KeyNotFoundException"></exception>
  public async Task<WfexsJob> Set(WfexsJob job)
  {
    var entity = await _db.WfexsJobs.FindAsync(job.Id) ?? throw new KeyNotFoundException();
    entity.Pid = job.Pid;
    entity.UnpackedPath = job.UnpackedPath;
    entity.WfexsRunId = job.WfexsRunId;
    entity.RunFinished = job.RunFinished;
    await _db.SaveChangesAsync();
    return entity;
  }

  /// <summary>
  /// Remove a <see cref="WfexsJob"/> with the given ID from the database.
  /// </summary>
  /// <param name="jobId">The ID of the <see cref="WfexsJob"/> to be removed from the database.</param>
  /// <returns></returns>
  public async Task Delete(int jobId)
  {
    var entry = await _db.WfexsJobs.FindAsync(jobId) ?? throw new KeyNotFoundException();
    _db.WfexsJobs.Remove(entry);
    await _db.SaveChangesAsync();
  }
}
