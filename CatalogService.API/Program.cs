using CatalogService.API.EndpointDefinitions;
using CatalogService.Core.Interfaces;
using CatalogService.Infrastructure;
using CatalogService.Infrastructure.Configurations;
using CatalogService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IItemRepository, ItemRepository>();
builder.Services.AddSingleton<ICategoryRepository, CategoryRepository>();

builder.Services.AddOptions();

builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.Configure<CatalogDatabaseConfiguration>(
    builder.Configuration.GetSection(CatalogDatabaseConfiguration.Name) );

var app = builder.Build();

CatalogEndpointDefinition.RegisterEndpoints(app);
await CatalogDatabase.Initiate(
    builder.Configuration["CatalogDatabase:ConnectionString"]!);

app.Run();
