using ClacksMiddleware.Extensions;
using HutchManager.Auth;
using HutchManager.Config;
using HutchManager.Constants;
using HutchManager.Data;
using HutchManager.Data.Entities.Identity;
using HutchManager.Extensions;
using HutchManager.HostedServices;
using HutchManager.Middleware;
using HutchManager.OptionsModels;
using HutchManager.Services;
using HutchManager.Services.QueryServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using Serilog;
using UoN.AspNetCore.VersionMiddleware;

var b = WebApplication.CreateBuilder(args);

b.Host.UseSerilog((context, services, loggerConfig) => loggerConfig
  .ReadFrom.Configuration(context.Configuration)
  .Enrich.FromLogContext());

#region Configure Services

// MVC
b.Services
  .AddControllersWithViews()
  .AddJsonOptions(DefaultJsonOptions.Configure);

// EF
b.Services
  .AddDbContext<ApplicationDbContext>(o =>
  {
    // migration bundles don't like null connection strings (yet)
    // https://github.com/dotnet/efcore/issues/26869
    // so if no connection string is set we register without one for now.
    // if running migrations, `--connection` should be set on the command line
    // in real environments, connection string should be set via config
    // all other cases will error when db access is attempted.
    var connectionString = b.Configuration.GetConnectionString("Default");
    if (string.IsNullOrWhiteSpace(connectionString))
      o.UseNpgsql();
    else
      o.UseNpgsql(connectionString,
        o => o.EnableRetryOnFailure());
  });

// Identity
b.Services
  .AddIdentity<ApplicationUser, IdentityRole>(
    o => o.SignIn.RequireConfirmedEmail = true)
  .AddClaimsPrincipalFactory<CustomClaimsPrincipalFactory>()
  .AddEntityFrameworkStores<ApplicationDbContext>()
  .AddDefaultTokenProviders();


b.Services
  .AddApplicationInsightsTelemetry()
  .ConfigureApplicationCookie(AuthConfiguration.IdentityCookieOptions)
  .AddAuthorization(AuthConfiguration.AuthOptions)
  .Configure<RegistrationOptions>(b.Configuration.GetSection("Registration"))
  .Configure<QueryQueueOptions>(b.Configuration.GetSection("JobQueue"))
  .Configure<RquestTaskApiOptions>(b.Configuration.GetSection("RquestTaskApi"))
  .Configure<RquestPollingServiceOptions>(b.Configuration)
  .AddEmailSender(b.Configuration)
  .AddTransient<UserService>()
  .AddTransient<FeatureFlagService>()
  .AddTransient<ActivitySourceService>()
  .AddTransient<DataSourceService>()
  .AddTransient<ResultsModifierService>()
  .AddTransient<QueryQueueService>()
  .AddHostedService<RquestPollingHostedService>()
  .AddScoped<IRquestPollingService, RquestPollingService>()
  .AddFeatureManagement();
b.Services
  .AddHttpClient<RquestTaskApiClient>();
#endregion

var app = b.Build();

#region Configure Pipeline
app.UseSerilogRequestLogging();
app.GnuTerryPratchett();

if (!app.Environment.IsDevelopment())
{
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseVersion();
app.UseConfigCookieMiddleware();

#endregion

#region Endpoint Routing

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Endpoints

app.MapControllers();

app.MapFallbackToFile("index.html").AllowAnonymous();

#endregion

app.Run();
