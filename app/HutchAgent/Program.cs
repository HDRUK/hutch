using HutchAgent.Config;
using HutchAgent.Services;

var builder = WebApplication.CreateBuilder(args);

#region Configure Service

builder.Services
  .Configure<MinioOptions>(builder.Configuration.GetSection("MinIO"))
  .Configure<WatchFolderOptions>(builder.Configuration.GetSection("WatchFolder"))
  .AddTransient<MinioService>()
  .AddHostedService<WatchFolderService>();

#endregion

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
