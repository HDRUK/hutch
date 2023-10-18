using System.IdentityModel.Tokens.Jwt;
using DummyControllerApi.Config;
using DummyControllerApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
  configuration.ReadFrom.Configuration(context.Configuration));

// Auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(
    opts =>
    {
      opts.TokenValidationParameters = new TokenValidationParameters
      {
        // We basically validate nothing about the token to avoid needing extra config about oidc.
        // We just want to confirm Hutch is sending an access token for a user.
        // Everything else will be environment setup dependent anyway
        ValidateActor = false,
        ValidateIssuer = false,
        ValidateIssuerSigningKey = false,
        ValidateLifetime = false,
        ValidateAudience = false,
        ValidateTokenReplay = false,
        RequireSignedTokens = false,
        SignatureValidator = (token, _) => new JwtSecurityToken(token),

        RequireExpirationTime = true,
        RequireAudience = true,
      };
    });
builder.Services.AddAuthorization();

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

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app
  .UseAuthentication()
  .UseAuthorization();

app.MapControllers().RequireAuthorization();

app.Run();
