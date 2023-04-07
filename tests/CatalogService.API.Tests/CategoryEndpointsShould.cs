using CatalogService.API.Models;
using CatalogService.Core.Models;
using FluentAssertions;
using System.Net.Http.Json;

namespace CatalogService.API.Tests
{
    public class CategoryEndpointsShould
    {
        [Test]
        [Order(1)]
        public async Task Add_new_category_successfully()
        {
            var client = TestHttpClientBuilder.CreateClient();
            var model = new CategoryModel("Stationery", null, null);

            var response = await client.PostAsync("/categories", 
                JsonContent.Create(model) );

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            var item = await response.Content.ReadFromJsonAsync<CategoryDetailResponse>();
            item.Links.Should().NotBeNull();
        }

        [Test]
        [Order(2)]
        public async Task Update_created_category_successfully()
        {
            var client = TestHttpClientBuilder.CreateClient();
            var model = new UpdateCategoryModel("Test", null, null);

            var response = await client.PutAsync("/categories/1",
                JsonContent.Create(model));

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Test]
        [Order(3)]
        public async Task List_all_categories()
        {
            var client = TestHttpClientBuilder.CreateClient();

            var response = await client.GetAsync("/categories");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var list = await response.Content.ReadFromJsonAsync<List<CategoryModel>>();

            list!.Count.Should().Be(1);
            list.First().Name.Should().Be("Test");
        }

        [Test]
        [Order(4)]
        public async Task Delete_category_successfully()
        {
            var client = TestHttpClientBuilder.CreateClient();
            var response = await client.DeleteAsync("/categories/1");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }
    }
}
