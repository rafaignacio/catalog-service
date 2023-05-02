using CatalogService.API.Core;
using CatalogService.API.Security;
using CatalogService.Infrastructure;
using CatalogService.Infrastructure.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAllEndpointDefinitions();
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
await CatalogDatabase.Initiate(
    builder.Configuration["CatalogDatabase:ConnectionString"]!);

app.Run();


public partial class Program { }