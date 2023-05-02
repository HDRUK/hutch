using HutchAgent.Config;

var builder = WebApplication.CreateBuilder(args);

#region Configure Service

builder.Services
  .Configure<MinioOptions>(builder.Configuration.GetSection("MinIO"));

#endregion

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
