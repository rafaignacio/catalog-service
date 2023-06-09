﻿using CatalogService.Core.Entities;
using CatalogService.Core.Validators;
using CatalogService.Infrastructure.Configurations;
using CatalogService.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace CatalogService.IntegrationTest;

[Order(1)]
public class CategoryShould
{
    private Category _sut = new Category(
            new CategoryValidator(),
            new CategoryRepository(
                Options.Create( new CatalogDatabaseConfiguration
                {
                    ConnectionString = Constants.ConnectionString
                })));

    [Test, Order(1)]
    public async Task Create_a_new_category_successfully()
    {
        var response = await _sut.Add(new("Stationery", null, null));
        response.IsT0.Should().BeTrue();
    }

    [Test, Order(2)]
    public async Task Add_a_new_child_category()
    {
        var response = await _sut.Add(new("Staples", "staples.png", "Stationery"));
        response.IsT0.Should().BeTrue();
    }

    [Test, Order(3)]
    public async Task Update_category_data_successfully()
    {
        var name = "Stationery";
        var image = "stationery.png";

        var response = await _sut.Update( new (1, name, image, null) );
        response.IsT0.Should().BeTrue();

        var category = await _sut.GetByName(name);
        category.IsT0.Should().BeTrue();
        category.AsT0.Image.Should().BeEquivalentTo(image);
    }

    [Test, Order(4)]
    public async Task Delete_category_successfully()
    {
        var response = await _sut.Delete(2);
        response.IsT0.Should().BeTrue();

        var category = await _sut.GetAll();
        category.IsT0.Should().BeTrue();
        category.AsT0.Count.Should().Be(1);
    }
}
