using FluentAssertions;
using Library.ControllerApi.DTOs;
using Library.ControllerApi.Services;
using Library.Data;
using Library.Data.Entities;
using Moq;

namespace Library.Tests.Unit;

public class InventoryServiceTests
{
    private readonly Mock<IInventoryRepository> _repo = new();

    private static InventoryItem Item(string sku, string name, int stock) =>
        new()
        {
            CurrentStock = stock,
            Product = new Product { Sku = sku, Name = name, Price = 10m }
        };

    [Fact]
    public async Task AllAsync_MockRepository()
    {
        //Arrange
        var items = new List<InventoryItem> { Item("BK-001", "The Clean COde", 5) };
        _repo.Setup(r => r.GetAllAsync()).ReturnsAsync(items);

        var sut = new InventoryService(_repo.Object);

        //Act
        var result = await sut.AllAsync();

        //Assert
        result.Should().BeSameAs(items);
        _repo.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task AddAsycn_UnpacksTheDTOIntoRepoArguments()
    {
        //Arrange
        var dto = new InventoryCreateDto("BK-009", "Domain-Driven Desgin", 54.99m, 4);

        _repo.Setup(r => r.AddInvetoryItemAsync("BK-009", "Domain-Driven Desgin", 54.99m, 4))
            .ReturnsAsync(Item("BK-009", "Domain-Driven Desgin", 4));

        var sut = new InventoryService(_repo.Object);

        //Act
        var created = await sut.AddAsync(dto);

        //Assert
        created.Product.Sku.Should().Be("BK-009");
        _repo.Verify(r => r.AddInvetoryItemAsync("BK-009", "Domain-Driven Desgin", 54.99m, 4)
            , Times.Once);
    }
}