using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Library.ControllerApi;
using Library.ControllerApi.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Library.Tests.Integration;

public class InventoryApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public InventoryApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetInventory_ContainsTheSeededCatalog()
    {
        //Arrange

        //Act
        var items = await _client.GetFromJsonAsync<List<InventoryDTO>>("/api/Inventory");

        //Assert
        items.Should().NotBeNullOrEmpty();
        items.Select(i => i.Sku).Should().Contain(["BK-001", "BK-002", "BK-003"]);
    }

    [Fact]
    public async Task GetBySku_UnknownSku_Returns404()
    {
        //Act
        var response = await _client.GetAsync("/api/Inventory/SOME-NONSENSE");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


}