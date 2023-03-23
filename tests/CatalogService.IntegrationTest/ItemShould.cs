using CatalogService.Core.Entities;
using CatalogService.Core.Validators;
using CatalogService.Infrastructure.Configurations;
using CatalogService.Infrastructure.Repositories;
using FluentAssertions;

namespace CatalogService.IntegrationTest;

[Order(2)]
public class ItemShould
{
    private Item _sut = new Item(
            new ItemValidator(),
            new ItemRepository(
                new CatalogDatabaseConfiguration
                {
                    ConnectionString = Constants.ConnectionString
                }));

    [Test, Order(1)]
    public async Task Create_a_new_item()
    {
        var response = await _sut.Add(new("Paper", null, null, "Stationery", 1.0, 1));
        response.IsT0.Should().BeTrue();
    }

    [Test, Order(2)]
    public async Task Update_item_data_successfully()
    {
        var name = "Paper";
        var description = "Recycable paper";
        var category = "Stationery";

        var response = await _sut.Update(new(name, description, null, category, 1.5, 10));
        response.IsT0.Should().BeTrue();

        var item = await _sut.GetByName(name);
        item.IsT0.Should().BeTrue();
        item.AsT0.Description.Should().BeEquivalentTo(description);
        item.AsT0.Price.Should().Be(1.5);
        item.AsT0.Amount.Should().Be(10);
    }

    [Test, Order(3)]
    public async Task Delete_item_successfully()
    {
        var name = "Paper";
        var response = await _sut.Delete(name);
        response.IsT0.Should().BeTrue();

        var category = await _sut.GetAll();
        category.IsT1.Should().BeTrue();
    }

    [Test, Order(4)]
    public async Task Indicate_failure_when_adding_item_to_an_unexisting_category()
    {
        var response = await _sut.Add(new("Mouse", null, null, "IT", 10.0, 100));
        response.IsT1.Should().BeTrue();
        response.AsT1.Message.Should().Contain("constraint failed");
    }
}
