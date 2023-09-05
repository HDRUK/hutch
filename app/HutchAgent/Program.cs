using HutchAgent.Config;
using HutchAgent.Constants;
using HutchAgent.Data;
using HutchAgent.Extensions;
using HutchAgent.HostedServices;
using HutchAgent.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

#region Configure Service

builder.Services
  .AddControllers()
  .AddJsonOptions(DefaultJsonOptions.Configure);


// Add DB context
builder.Services.AddDbContext<HutchAgentContext>(o =>
{
  var connectionString = builder.Configuration.GetConnectionString("AgentDb");
  o.UseSqlite(connectionString);
});

// All other services
builder.Services
  .Configure<PathOptions>(builder.Configuration.GetSection("Paths"))

  .Configure<RabbitQueueOptions>(builder.Configuration.GetSection("Queue"))
  .Configure<JobActionsQueueOptions>(builder.Configuration.GetSection("Queue"))

  .Configure<MinioOptions>(builder.Configuration.GetSection("MinIO"))
  .Configure<WorkflowTriggerOptions>(builder.Configuration.GetSection("Wfexs"))
  .Configure<PublisherOptions>(builder.Configuration.GetSection("Publisher"))

  .AddScoped<WorkflowTriggerService>()
  .AddTransient<WorkflowFetchingService>()
  .AddResultsStore(builder.Configuration)
  .AddTransient<WorkflowJobService>()
  .AddHostedService<QueuePollingHostedService>()

  .AddTransient<CrateService>()
  .AddSingleton<BagItService>()
  .AddTransient<IQueueWriter, RabbitQueueWriter>()
  .AddTransient<IQueueReader, RabbitQueueReader>()

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
