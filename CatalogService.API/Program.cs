using CatalogService.API.Core;
using CatalogService.Infrastructure;
using CatalogService.Infrastructure.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAllEndpointDefinitions();
builder.Services.AddOptions();

builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.Configure<CatalogDatabaseConfiguration>(
    builder.Configuration.GetSection(CatalogDatabaseConfiguration.Name) );

var app = builder.Build();

app.UseEndpointDefinitions();
await CatalogDatabase.Initiate(
    builder.Configuration["CatalogDatabase:ConnectionString"]!);

app.Run();


public partial class Program { }