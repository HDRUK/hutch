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

builder.Services.AddFeatureManagement();

builder.Services.AddSwaggerGen();

// All other services
builder.Services
  .Configure<PathOptions>(builder.Configuration.GetSection("Paths"))
  .Configure<RabbitQueueOptions>(builder.Configuration.GetSection("Queue"))
  .Configure<JobActionsQueueOptions>(builder.Configuration.GetSection("Queue"))
  .Configure<MinioOptions>(builder.Configuration.GetSection("MinIO"))
  .Configure<WorkflowTriggerOptions>(builder.Configuration.GetSection("Wfexs"))
  .Configure<PublisherOptions>(builder.Configuration.GetSection("Publisher"))
  .Configure<LicenseOptions>(builder.Configuration.GetSection("License"))
  .AddTransient<WorkflowTriggerService>()
  .AddTransient<WorkflowFetchService>()
  .AddResultsStore(builder.Configuration)
  .AddTransient<WorkflowJobService>()
  .AddHostedService<QueuePollingHostedService>()
  .AddScoped<ExecuteActionHandler>()
  .AddTransient<CrateService>()
  .AddSingleton<BagItService>()
  .AddTransient<IQueueWriter, RabbitQueueWriter>()
  .AddTransient<IQueueReader, RabbitQueueReader>()
  .AddScoped<FinaliseActionHandler>();
  

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
