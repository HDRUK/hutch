using HutchAgent.Models;
using HutchAgent.Services;
using HutchAgent.Config;

var builder = WebApplication.CreateBuilder(args);

#region Configure Service

builder.Services
  .AddControllersWithViews();
builder.Services
  .Configure<MinioOptions>(builder.Configuration.GetSection("MinIO"))
  .Configure<WorkflowTriggerOptions>(builder.Configuration.GetSection("Wfexs"))
  .AddScoped<WorkflowTriggerService>()
  .AddTransient<MinioService>();

#endregion

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();
