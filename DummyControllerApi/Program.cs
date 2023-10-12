using DummyControllerApi.Config;
using DummyControllerApi.Services;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


// Auth
builder.Services.AddAuthentication()
  .AddJwtBearer(
    opts =>
    {
      opts.TokenValidationParameters = new TokenValidationParameters
      {
        // TODO we're not interested in actually checking in with whatever idp is in use
        // we just want to confirm Hutch is sending a token
        // everything else will be environment setup dependent anyway
        ValidateIssuer = false,
        ValidateIssuerSigningKey = false,
        RequireExpirationTime = true
      };
    });
builder.Services.AddAuthorization(o =>
{
  //o.DefaultPolicy = ;
});

// Configure Options Models
builder.Services.Configure<EgressBucketDetailsOptions>(builder.Configuration.GetSection("EgressBucketDetails"));

// MVC and stuff
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Application Services

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

app
  .UseAuthentication()
  .UseAuthorization();

app.MapControllers().RequireAuthorization();

app.Run();
