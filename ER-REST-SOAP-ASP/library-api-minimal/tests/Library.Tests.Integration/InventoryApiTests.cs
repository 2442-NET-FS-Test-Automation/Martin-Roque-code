using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Library.ControllerApi.DTOs;

namespace Library.Tests.Integration;

public class InventoryApiTests : IClassFixture<LibraryApiFactory>
{
    private readonly HttpClient _client;

    public InventoryApiTests(LibraryApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private record TokenResponse(string token);

    private async Task<HttpClient> AsAdminAsync()
    {
        var login = await _client.PostAsJsonAsync("/auth/login", new { username = "ada", password = "pass123!" });
        var token = (await login.Content.ReadFromJsonAsync<TokenResponse>())!.token;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return _client;
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

    [Fact]
    public async Task GetSupplierPrice_UsesTheFakeSupplier()
    {
        //Arrange
        var client = await AsAdminAsync();

        //Act
        var response = await client.GetAsync("/api/Inventory/BK-001/supplier-price");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<SupplierPriceResponse>();
        body!.supplierPrice.Should().Be(99.99m);
    }

    private record SupplierPriceResponse(string sku, decimal supplierPrice);
}