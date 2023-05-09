using HutchAgent.Config;
using HutchAgent.Data;
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
  .Configure<WatchFolderOptions>(builder.Configuration.GetSection("WatchFolder"))
  .Configure<WorkflowTriggerOptions>(builder.Configuration.GetSection("Wfexs"))
  .AddScoped<WorkflowTriggerService>()
  .AddTransient<MinioService>()
  .AddTransient<SqliteService>()
  .AddHostedService<WatchFolderService>();

#endregion

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();
