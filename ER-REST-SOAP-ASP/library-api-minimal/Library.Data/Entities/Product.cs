using Microsoft.EntityFrameworkCore;

namespace Library.Data.Entities;

public class Product
{
    public int Id { get; set; }
    public string? Sku { get; set; }
    public string? Name { get; set; }
    [Precision(10, 2)]
    public decimal Price { get; set; }

    //Example of using a collection to denote a relationship
    public InventoryItem? Inventory { get; set; }
}