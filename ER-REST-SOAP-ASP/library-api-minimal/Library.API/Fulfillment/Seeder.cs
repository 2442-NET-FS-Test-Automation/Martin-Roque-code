using Library.Data;
using Library.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Identity.Client;

//In "production" our orders would come from users. These API's runs locally

public interface ISeeder
{
    IReadOnlyList<int> SeedOrders(int n, bool expedited);
}

public class Seeder : ISeeder
{
    public static readonly string[] Skus = { "BK-001", "BK-002", "BK-003" };
    private readonly IDbContextFactory<LibraryDbContext> _factory;

    public Seeder(IDbContextFactory<LibraryDbContext> factory)
    {
        _factory = factory;
    }

    public IReadOnlyList<int> SeedOrders(int n, bool expedited)
    {
        using var db = _factory.CreateDbContext();

        //create a dictionary based on our product table and skus
        var pid = db.Products.ToDictionary(p => p.Sku, p => p.Id);

        var ids = new List<int>(n);

        for (int i = 0; i < n; i++)
        {
            var order = new Order
            {
                CustomerId = Random.Shared.Next(1, 3),
                Priority = expedited ? Priority.Expedited : Priority.Normal,
                Lines = { new OrderLine { ProductId = pid[Skus[i % Skus.Length]], Quantity = 1 } }
            };

            db.Orders.Add(order);
            db.SaveChanges();
        }
        return ids;
    }
}