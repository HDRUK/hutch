using HutchAgent.Models;
using HutchAgent.Services;

var b = WebApplication.CreateBuilder(args);

b.Services
  .AddControllersWithViews();
b.Services
  .Configure<WorkflowTriggerOptions>(b.Configuration.GetSection("Wfexs"))
  .AddScoped<WorkflowTriggerService>();

var app = b.Build();

app.UseRouting();
app.MapControllers();

app.Run();
