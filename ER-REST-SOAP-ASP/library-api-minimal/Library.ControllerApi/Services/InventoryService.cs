using Library.Data;
using Library.Data.Entities;

namespace Library.ControllerApi.Services;

public class InventoryService : IInventoryService
{
    // Our InventoryService is what will call repo layer methods, so it
    // gets that dependency. Not the controller layer
    private readonly IInventoryRepository _repo;

    public InventoryService(IInventoryRepository repo)
    {
        _repo = repo;
    }

    public Task<IReadOnlyList<InventoryItem>> AllAsync()
    {
        return _repo.GetAllAsync();
    }

    public Task<InventoryItem?> BySkuAsync(string sku)
    {
        return _repo.GetInventoryItemBySkuAsync(sku);
    }

    // public Task<InventoryItem> AddAsync()
    // {
    //     return _repo.AddInvetoryItemAsync();
    // }

    public Task<bool> RemoveAsync(string sku)
    {
        return _repo.RemoveBySkuAsync(sku);
    }
}