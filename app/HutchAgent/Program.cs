using HutchAgent.Config;
using HutchAgent.Data;
using HutchAgent.Extensions;
using HutchAgent.HostedServices;
using HutchAgent.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

#region Configure Service

builder.Services
  .AddControllersWithViews();

// Add DB context
builder.Services.AddDbContext<HutchAgentContext>(o =>
{
  var connectionString = builder.Configuration.GetConnectionString("AgentDb");
  o.UseSqlite(connectionString);
});

// All other services
builder.Services
  .Configure<MinioOptions>(builder.Configuration.GetSection("MinIO"))
  .Configure<JobPollingOptions>(builder.Configuration.GetSection("WatchFolder"))
  .Configure<WorkflowTriggerOptions>(builder.Configuration.GetSection("Wfexs"))
  .AddScoped<WorkflowTriggerService>()
  .AddResultsStore(builder.Configuration)
  .AddTransient<WfexsJobService>()
  .AddTransient<CrateMergerService>()
  .AddHostedService<JobPollingHostedService>();

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
