using FluentAssertions;
using Library.Data;
using Library.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Library.Tests.Integration;

public class LiveDatabaseTests : IDisposable
{
    private const string LiveConnection =
        "Server=localhost,1433;Database=LibraryMinimalDb;User ID=sa;Password=LibPass123;TrustServerCertificate=true";

    private readonly LibraryDbContext _db;
    private IDbContextTransaction _tx;

    public LiveDatabaseTests()
    {
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseSqlServer(LiveConnection)
            .Options;

        _db = new LibraryDbContext(options);

        _tx = _db.Database.BeginTransaction();
    }

    public void Dispose()
    {
        _tx.Rollback();
        _tx.Dispose();
        _db.Dispose();
    }

    [Fact]
    public async Task SeedCatalog_IsPresentInTheLiveDatabase()
    {
        //Assert
        var skus = await _db.Products.Select(p => p.Sku).ToListAsync();

        skus.Should().Contain(["BK-001", "BK-002", "BK-003"]);
    }

    [Fact]
    public async Task AddedProduct_IsVisibleTransaction_DeletedUponRollBack()
    {
        //Act
        _db.Products.Add(new Product
        {
            Sku = "TX-TEST-001",
            Name = "Rollback Book",
            Price = 1.00m
        });
        await _db.SaveChangesAsync();

        //Assert
        (await _db.Products.CountAsync(p => p.Sku == "TX-TEST-001")).Should().Be(1);
    }
}