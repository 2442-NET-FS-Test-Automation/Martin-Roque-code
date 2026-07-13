namespace Library.ControllerApi.Services;

public class SupplierClient : ISupplierClient
{
    // Calling an outside API using HTTP Client
    private readonly HttpClient _http;

    public SupplierClient(HttpClient http)
    {
        _http = http;
    }

    //Represent the response "shape" of that outside API
    private record SupplierProduct(int Id, string Title, decimal Price);

    public async Task<decimal?> GetListPriceAsync(string sku)
    {
        var digits = new string(sku.Where(char.IsDigit).ToArray());

        if (!int.TryParse(digits, out var id)) return null;

        var product = await _http.GetFromJsonAsync<SupplierProduct>($"products/{id}");

        return product?.Price;
    }
}