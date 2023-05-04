using HutchAgent.Services;
using HutchAgent.Config;

var builder = WebApplication.CreateBuilder(args);

#region Configure Service

builder.Services
  .AddControllersWithViews();
builder.Services
  .Configure<MinioOptions>(builder.Configuration.GetSection("MinIO"))
  .Configure<WatchFolderOptions>(builder.Configuration.GetSection("WatchFolder"))
  .Configure<WorkflowTriggerOptions>(builder.Configuration.GetSection("WorkflowTriggering"))
  .AddTransient<MinioService>()
  .AddHostedService<WatchFolderService>()
  .AddHostedService<WorkflowTriggerService>();

#endregion

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();
