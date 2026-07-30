using Library.ControllerApi.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Tests.Integration;

public class LibraryApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            //Appending to the builder.services from API's Program.cs
            services.AddSingleton<ISupplierClient, FakeSupplierClient>();
        });
    }
}

//Deterministic stand-in (fake) for the external supplier API

public class FakeSupplierClient : ISupplierClient
{
    public Task<decimal?> GetListPriceAsync(string sku)
    {
        return Task.FromResult<decimal?>(99.99m);
    }
}

[CollectionDefinition("Library API")]
public class LibraryApiCollection : ICollectionFixture<LibraryApiFactory>;