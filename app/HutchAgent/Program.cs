using HutchAgent.Config;
using HutchAgent.Services;

var builder = WebApplication.CreateBuilder(args);

#region Configure Service

builder.Services
  .AddControllersWithViews();
builder.Services
  .Configure<MinioOptions>(builder.Configuration.GetSection("MinIO"))
  .Configure<WatchFolderOptions>(builder.Configuration.GetSection("WatchFolder"))
  .Configure<WorkflowTriggerOptions>(builder.Configuration.GetSection("Wfexs"))
  .AddScoped<WorkflowTriggerService>()
  .AddTransient<MinioService>()
  .AddHostedService<WatchFolderService>();

#endregion

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();
