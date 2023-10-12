using DummyControllerApi.Config;
using DummyControllerApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<EgressBucketDetailsOptions>(builder.Configuration.GetSection("EgressBucketDetails"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Use this combo for delaying approvals while running
builder.Services
  .AddSingleton<InMemoryApprovalQueue>()
  .AddHostedService<EgressApprovalHostedService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
