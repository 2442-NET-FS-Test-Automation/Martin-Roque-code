using Library.Data;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]

public class InventoryController : ControllerBase
{
    private readonly IInventoryRepository _repo;

    public InventoryController(IInventoryRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<ActionResult<EntireInventoryDTO>> Get()
    {
        //return Ok(await _repo.GetAllAsync());

        //Using a DTO - Data Transfer Object
        var items = await _repo.GetAllAsync();
        // Now we need to map to those DTOs
        EntireInventoryDTO response = new();

        foreach (var item in items)
        {
            InventoryReturnDTO i = new InventoryReturnDTO
            {
                Name = item.Product.Name,
                Sku = item.Product.Sku,
                CurrentStock = item.CurrentStock
            };

            response.EntireInventory.Add(i);
        }

        return Ok(response);
    }

    [HttpGet("{sku}")]
    public async Task<ActionResult<InventoryReturnDTO>> GetBySku(string sku)
    {
        var item = await _repo.GetInventoryItemBySkuAsync(sku);

        if (item is null)
            return NotFound();
        var response = new InventoryReturnDTO
        {
            Name = item.Product.Name,
            Sku = item.Product.Sku,
            CurrentStock = item.CurrentStock
        };


        return Ok(response);
    }
}