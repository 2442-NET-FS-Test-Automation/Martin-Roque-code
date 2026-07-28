using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using System.Net.Http.Headers;
using Library.ControllerApi.DTOs;

namespace Library.Tests.Integration;

public class AuthApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthApiTests(WebApplicationFactory<Program> factory)
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
    public async Task Login_WithSeededAdmin_ReturnsToken()
    {
        //Arrange
        var body = new { username = "ada", password = "pass123!" };

        //Act
        var response = await _client.PostAsJsonAsync("/auth/login", body);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<TokenResponse>();
        payload!.token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401()
    {
        //Arrange
        var body = new { username = "ada", password = "wrong-password" };

        //Act
        var response = await _client.PostAsJsonAsync("/auth/login", body);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PostInventory_WithoutToken_Returns401()
    {
        //Arrange
        var dto = new InventoryCreateDto("BK-401", "No Token", 10.00m, 1);

        //Act
        var response = await _client.PostAsJsonAsync("/api/Inventory", dto);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PostInventory_AsAdmin_Creates()
    {
        //Arrange
        var client = await AsAdminAsync();
        var dto = new InventoryCreateDto("BK-TEST-INT", "Integration Test Book", 21.50m, 2);

        //Act
        var created = await client.PostAsJsonAsync("/api/Inventory", dto);

        //Assert
        created.StatusCode.Should().Be(HttpStatusCode.Created);
        created.Headers.Location!.ToString().Should().Contain("BK-TEST-INT");

        //Cleanup - Very importan in integration testing (and really anything adove unit testing)
        var deleted = await client.DeleteAsync("api/Inventory/BK-TEST-INT");
        deleted.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}