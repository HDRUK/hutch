using System.IO.Abstractions;
using System.Reflection;
using Flurl.Http.Configuration;
using HutchAgent.Config;
using HutchAgent.Constants;
using HutchAgent.Data;
using HutchAgent.Extensions;
using HutchAgent.Services;
using HutchAgent.Services.ActionHandlers;
using HutchAgent.Services.Contracts;
using HutchAgent.Services.Hosted;
using HutchAgent.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => 
  configuration.ReadFrom.Configuration(context.Configuration));


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

builder.Services.AddFeatureManagement(
  builder.Configuration.GetSection("Flags"));

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

  o.EnableAnnotations();
  o.SupportNonNullableReferenceTypes();
});

// Other Services
builder.Services
  .AddAutoMapper(typeof(Program).Assembly)
  .AddSingleton<IFlurlClientFactory, PerBaseUrlFlurlClientFactory>()
  .AddHttpClient()  // We prefer Flurl for most use cases, but IdentityModel has extensions for vanilla HttpClient
  .AddTransient<IFileSystem,FileSystem>();

// IOptions
builder.Services
  .Configure<PathOptions>(builder.Configuration.GetSection("Paths"))
  .Configure<RabbitQueueOptions>(builder.Configuration.GetSection("Queue"))
  .Configure<JobActionsQueueOptions>(builder.Configuration.GetSection("Queue"))
  .Configure<MinioOptions>(builder.Configuration.GetSection("StoreDefaults"))
  .Configure<WorkflowTriggerOptions>(builder.Configuration.GetSection("WorkflowExecutor"))
  .Configure<CratePublishingOptions>(builder.Configuration.GetSection("CratePublishing"))
  .Configure<ControllerApiOptions>(builder.Configuration.GetSection("ControllerApi"))
  .Configure<OpenIdOptions>(builder.Configuration.GetSection("IdentityProvider"));

// JobAction Handlers
builder.Services
  .AddScoped<FetchAndExecuteActionHandler>()
  .AddScoped<ExecuteActionHandler>()
  .AddScoped<InitiateEgressActionHandler>()
  .AddScoped<FinalizeActionHandler>();

// Hosted Services
builder.Services
  .AddHostedService<JobActionQueuePoller>();

// Other Application Services
builder.Services
  .AddTransient<FileSystemUtility>()
  .AddTransient<JobLifecycleService>()
  .AddTransient<StatusReportingService>()
  .AddTransient<WorkflowJobService>()
  .AddTransient<ControllerApiService>()
  .AddSingleton<BagItService>()
  .AddTransient<FiveSafesCrateService>()
  .AddTransient<WorkflowTriggerService>()
  .AddTransient<WorkflowFetchService>()
  .AddIntermediaryStoreFactory(builder.Configuration)
  .AddTransient<OpenIdIdentityService>()
  .AddTransient<IQueueWriter, RabbitQueueWriter>()
  .AddTransient<IQueueReader, RabbitQueueReader>();

#endregion

var app = builder.Build();

app.UseSerilogRequestLogging();

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
