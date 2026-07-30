using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Library.ControllerApi.DTOs;

namespace Library.Tests.Integration;

[Collection("Library API")]

public class ModelValidationTests
{
    private readonly HttpClient _client;

    public ModelValidationTests(LibraryApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private record TokenResposne(string token);

    [Fact]
    public void DirectValidator_MissesPositionalRecordAttributes()
    {
        //Arrange
        var dto = new InventoryCreateDto("BK-BAD", "Bad Book", -50.00m, 1);
        var results = new List<ValidationResult>();

        //Act
        var valid = Validator.TryValidateObject(dto, new ValidationContext(dto),
            results, validateAllProperties: true);

        //Asserts
        valid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Fact]
    public async Task PostInventory_WithInvalidBody_Returns400()
    {
        //Arrange
        var login = await _client.PostAsJsonAsync("/auth/login",
            new { username = "ada", password = "pass123!" });
        var token = (await login.Content.ReadFromJsonAsync<TokenResposne>())!.token;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var dto = new InventoryCreateDto("BK-BAD", "Bad Book", -50.00m, 1);

        //Act
        var response = await _client.PostAsJsonAsync("/api/Inventory", dto);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

    }
}