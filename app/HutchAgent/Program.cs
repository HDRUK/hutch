using HutchAgent.Config;
using HutchAgent.Services;

var builder = WebApplication.CreateBuilder(args);

#region Configure Service

builder.Services
  .Configure<MinioOptions>(builder.Configuration.GetSection("MinIO"))
  .Configure<MinioOptions>(builder.Configuration.GetSection("WatchFolder"))
  .AddTransient<MinioService>();

#endregion

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
