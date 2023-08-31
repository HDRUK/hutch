using HutchAgent.Config;
using HutchAgent.Data;
using HutchAgent.Extensions;
using HutchAgent.HostedServices;
using HutchAgent.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

#region Configure Service

builder.Services
  .AddControllers();

// Add DB context
builder.Services.AddDbContext<HutchAgentContext>(o =>
{
  var connectionString = builder.Configuration.GetConnectionString("AgentDb");
  o.UseSqlite(connectionString);
});

// All other services
builder.Services
  .Configure<PathOptions>(builder.Configuration.GetSection("Paths"))

  .Configure<MinioOptions>(builder.Configuration.GetSection("MinIO"))
  .Configure<JobPollingOptions>(builder.Configuration.GetSection("WatchFolder"))
  .Configure<WorkflowTriggerOptions>(builder.Configuration.GetSection("Wfexs"))
  .Configure<PublisherOptions>(builder.Configuration.GetSection("Publisher"))

  .AddScoped<WorkflowTriggerService>()
  .AddResultsStore(builder.Configuration)
  .AddTransient<WfexsJobService>()
  .AddTransient<CrateService>()
  .AddHostedService<JobPollingHostedService>()
  .AddSingleton<Sha512ChecksumService>()
  .AddSingleton<BagitChecksumWriter>()
  .AddFeatureManagement();

#endregion

var app = builder.Build();

app.UseRouting();
app.MapControllers();

#region Automatic Migrations

using (var scope = app.Services.CreateScope())
{
  var dbContext = scope.ServiceProvider.GetRequiredService<HutchAgentContext>();

  dbContext.Database.Migrate();
}

#endregion

app.Run();
