using CatalogService.API.Core;
using CatalogService.API.Security;
using CatalogService.Infrastructure;
using CatalogService.Infrastructure.Configurations;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAllEndpointDefinitions();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog v1", Version = "v1" });
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
      {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
              {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
              },
              Scheme = "oauth2",
              Name = "Bearer",
              In = ParameterLocation.Header,

            },
            new List<string>()
          }
        });
});

builder.Services.AddOptions();

builder.Services.AddAuthSecurity( builder.Configuration );

builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.Configure<CatalogDatabaseConfiguration>(
    builder.Configuration.GetSection(CatalogDatabaseConfiguration.Name) );

builder.Services.Configure<QueueConfiguration>(
    builder.Configuration.GetSection(QueueConfiguration.Name));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpointDefinitions();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("./v1/swagger.json", "v1");
});

await CatalogDatabase.Initiate(
    builder.Configuration["CatalogDatabase:ConnectionString"]!);

app.Run();


public partial class Program { }