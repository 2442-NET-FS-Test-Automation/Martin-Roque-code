using AutoMapper;
using FluentAssertions;
using Library.ControllerApi.Services;
using Library.Data.Entities;
using Library.Tests.Unit.Fixtures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace Library.Tests.Unit;

public class InventoryControllerTests : IClassFixture<MapperFixture>
{
    private readonly Mock<IInventoryService> _service = new();
    private readonly Mock<ISupplierClient> _supplier = new();
    private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    private readonly IMapper _mapper;

    public InventoryControllerTests(MapperFixture mFixture)
    {
        // var config = new MapperConfiguration(cfg =>
        //     cfg.AddProfile<MappingProfile>(), NullLoggerFactory.Instance);

        // _mapper = config.CreateMapper();
        _mapper = mFixture.Mapper;
    }

    private InventoryController CreateSut() =>
        new(_service.Object, _mapper, _cache, _supplier.Object);

    private static InventoryItem Item(string sku, string name, int stock) =>
        new()
        {
            CurrentStock = stock,
            Product = new Product { Sku = sku, Name = name, Price = 10m }
        };

    [Fact]
    public async Task Get_ReturnsOkWithMappedDtos()
    {
        //Arrange
        _service.Setup(s => s.AllAsync())
            .ReturnsAsync(new List<InventoryItem> { Item("BK-001", "Clean Code", 5) });

        var sut = CreateSut();
        //Act
        var result = await sut.Get();

        //Assert
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(new[] { new { Sku = "BK-001", Name = "Clean Code", CurrentStock = 5 } });
    }

    [Fact]
    public async Task Get_SecondCall_ServersFromCache_ServiceCalledOnce()
    {
        //Arrange
        _service.Setup(s => s.AllAsync())
            .ReturnsAsync(new List<InventoryItem> { Item("BK-001", "Clean Code", 5) });

        var sut = CreateSut();
        //Act
        await sut.Get();
        await sut.Get();

        //Assert
        _service.Verify(s => s.AllAsync(), Times.Once);
    }
}