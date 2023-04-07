using CatalogService.Infrastructure.Configurations;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CatalogService.API.Tests;

public static class TestHttpClientBuilder
{
    public static HttpClient CreateClient()
    {
        var web = new WebApplicationFactory<Program>();

        return web.WithWebHostBuilder( builder =>
        {
            //builder.ConfigureAppConfiguration((context, configBuilder) =>
            //{
            //    configBuilder.AddInMemoryCollection(
            //        new Dictionary<string, string?>
            //        {
            //            ["CatalogDatabase:ConnectionString"] = "Data Source=Catalog.db;Mode=Memory;Cache=Shared"
            //        });
            //});

            //builder.ConfigureTestServices(services =>
            //{
            //    services.Configure<CatalogDatabaseConfiguration>(opt =>
            //    {
            //        opt.ConnectionString = "Data Source=Catalog.db;Mode=Memory;Cache=Shared";
            //    });
            //});
            builder.UseTestServer();
        }).CreateClient();
    }
}
