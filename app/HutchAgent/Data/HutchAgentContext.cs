using HutchAgent.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HutchAgent.Data;

public class HutchAgentContext : DbContext
{
  public HutchAgentContext(DbContextOptions<HutchAgentContext> options) : base(options)
  {
  }

  public DbSet<WfexsJob> WfexsJobs => Set<WfexsJob>();
}
