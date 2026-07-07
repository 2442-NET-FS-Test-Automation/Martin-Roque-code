namespace Library.API.Fulfillment;

public sealed class UnknownSkuException : Exception
{
    public string Sku { get; } = default!;

    public UnknownSkuException(string sku) : base($"Unknown SKU: {sku}")
    {
        Sku = sku;
    }
}