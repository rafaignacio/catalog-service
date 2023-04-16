using CatalogService.API.Models;
using CatalogService.Core.Entities;
using CatalogService.Core.Models;
using FluentAssertions;
using System.Net.Http.Json;

namespace CatalogService.API.Tests;

[Order(1)]
public class ItemsEndpointsShould
{
    [Test]
    [Order(1)]
    public async Task Add_new_item_successfully()
    {
        var client = TestHttpClientBuilder.CreateClient();
        var model = new AddItemModel
        {
            Name = "Paper",
            Category = "Test",
            Amount = 1,
            Price = 10
        };

        var response = await client.PostAsync("/items",
            JsonContent.Create(model));

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        var item = await response.Content.ReadFromJsonAsync<ItemDetailResponse>();
        item.Links.Should().NotBeNull();
    }

    [Test]
    [Order(2)]
    public async Task Update_created_item_successfully()
    {
        var client = TestHttpClientBuilder.CreateClient();
        var model = new UpdateItemModel(null, "testing update", null, null, 0, 0);

        var response = await client.PutAsync("/items/1",
            JsonContent.Create(model));
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    }

    [Test]
    [Order(3)]
    public async Task List_all_items()
    {
        var client = TestHttpClientBuilder.CreateClient();

        var response = await client.GetAsync("/items");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<ItemModel>>();

        list!.Count.Should().Be(1);
        list.First().Description.Should().Be("testing update");
    }

    [Test]
    [Order(4)]
    public async Task Delete_category_successfully_and_remove_items()
    {
        var client = TestHttpClientBuilder.CreateClient();
        var response = await client.DeleteAsync("/categories/1");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);

        response = await client.GetAsync("/items");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<ItemModel>>();
        list!.Count.Should().Be(0);
    }
}
