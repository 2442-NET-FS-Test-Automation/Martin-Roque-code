using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using Library.ControllerApi.Services;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace Library.Tests.Unit;

public class TokenServiceTests
{
    private readonly ITestOutputHelper _output;

    private const string TestKey = "unit-test-signing-key-32-bytes-min!!";

    public TokenServiceTests(ITestOutputHelper output)
    {
        _output = output;
    }

    private static TokenService CreateSut()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            { ["Jwt:key"] = TestKey })
            .Build();

        return new TokenService(config);
    }

    [Fact]
    public void Issue_ReturnsParsableJwt()
    {
        //Arrange
        var sut = CreateSut();

        //Act
        var token = sut.Issue("ada", "admin");
        _output.WriteLine(token);

        //Assert
        //Fluent assertions
        var parsed = new JwtSecurityTokenHandler().ReadJwtToken(token);
        parsed.Issuer.Should().Be("library-fulfillment");
        parsed.Audiences.Should().Contain("library-fulfillment-clients");

        //Base Xunit
        Assert.Equal("library-fulfillment", parsed.Issuer);
        Assert.Contains("library-fulfillment-clients", parsed.Audiences);
    }

    // Test to make sure we are getting Name/Role
    [Fact]
    public void Issue_IncludesNameAndRoleClaims()
    {
        //Arrange
        var sut = CreateSut();

        //Act
        var token = sut.Issue("ada", "admin");

        //Assert
        var parsed = new JwtSecurityTokenHandler().ReadJwtToken(token);

        parsed.Claims.Should().Contain(c =>
            c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
                && c.Value == "ada");

        parsed.Claims.Should().Contain(c =>
            c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                && c.Value == "admin");
    }

    // Fact takes no inputs, Theory takes input
    [Theory]
    [InlineData("ada", "admin")]
    [InlineData("grace", "consumer")]
    public void Issue_SetsRoleClaims_ForAnyRole(string user, string role)
    {
        var sut = CreateSut();

        //Act
        var token = sut.Issue(user, role);

        //Assert
        var parsed = new JwtSecurityTokenHandler().ReadJwtToken(token);

        parsed.Claims.Should().Contain(c =>
            c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                && c.Value == role);
    }
}