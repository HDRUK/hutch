using System.Reflection;
using HutchAgent.Config;
using HutchAgent.Constants;
using HutchAgent.Data;
using HutchAgent.Extensions;
using HutchAgent.HostedServices;
using HutchAgent.Services;
using HutchAgent.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

#region Configure Service

builder.Services
  .AddControllers()
  .AddJsonOptions(DefaultJsonOptions.Configure);


// Add DB context
builder.Services.AddDbContext<HutchAgentContext>(o =>
{
  var connectionString = builder.Configuration.GetConnectionString("AgentDb");
  o.UseSqlite(connectionString ?? "Data Source=hutch-agent.db");
});

builder.Services.AddFeatureManagement();

builder.Services.AddSwaggerGen(o =>
{
  o.SwaggerDoc("v1", new OpenApiInfo
  {
    Version = "v1",
    Title = "Hutch Agent API",
    Description = "A REST API for interacting with the Hutch Agent",
    License = new OpenApiLicense
    {
      Name = "MIT License",
      Url = new Uri("https://github.com/HDRUK/hutch/blob/main/LICENSE")
    }
  });

  o.IncludeXmlComments(Path.Combine(
    AppContext.BaseDirectory,
    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

// IOptions
builder.Services
  .Configure<PathOptions>(builder.Configuration.GetSection("Paths"))
  .Configure<RabbitQueueOptions>(builder.Configuration.GetSection("Queue"))
  .Configure<JobActionsQueueOptions>(builder.Configuration.GetSection("Queue"))
  .Configure<MinioOptions>(builder.Configuration.GetSection("MinIO"))
  .Configure<WorkflowTriggerOptions>(builder.Configuration.GetSection("Wfexs"))
  .Configure<PublisherOptions>(builder.Configuration.GetSection("Publisher"))
  .Configure<LicenseOptions>(builder.Configuration.GetSection("License"));

// JobAction Handlers
builder.Services
  .AddScoped<ExecuteActionHandler>()
  .AddScoped<FinaliseActionHandler>();

// Hosted Services
builder.Services
  .AddHostedService<QueuePollingHostedService>();

// Other Application Services
builder.Services
  .AddTransient<StatusReportingService>()
  .AddTransient<WorkflowTriggerService>()
  .AddTransient<WorkflowFetchService>()
  .AddResultsStore(builder.Configuration)
  .AddTransient<WorkflowJobService>()
  .AddTransient<CrateService>()
  .AddSingleton<BagItService>()
  .AddTransient<IQueueWriter, RabbitQueueWriter>()
  .AddTransient<IQueueReader, RabbitQueueReader>();

#endregion

var app = builder.Build();

app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
  options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
  options.RoutePrefix = string.Empty;
});
app.MapControllers();

#region Automatic Migrations

using (var scope = app.Services.CreateScope())
{
  var dbContext = scope.ServiceProvider.GetRequiredService<HutchAgentContext>();

  dbContext.Database.Migrate();
}

#endregion

app.Run();
