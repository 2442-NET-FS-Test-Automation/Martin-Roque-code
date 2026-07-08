using AutoMapper;
using Library.ControllerApi.DTOs;
using Library.ControllerApi.Services;
using Library.Data;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]

public class InventoryController : ControllerBase
{
    private readonly IInventoryService _service;
    private readonly IMapper _mapper;

    public InventoryController(IInventoryService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryDTO>>> Get()
    {

        var items = await _service.AllAsync();

        var mappedItems = _mapper.Map<List<InventoryDTO>>(items);

        return Ok(mappedItems);
        //return Ok(await _repo.GetAllAsync());

        //Using a DTO - Data Transfer Object
        // var items = await _repo.GetAllAsync();
        // // Now we need to map to those DTOs
        // EntireInventoryDTO response = new();

        // foreach (var item in items)
        // {
        //     InventoryReturnDTO i = new InventoryReturnDTO
        //     {
        //         Name = item.Product.Name,
        //         Sku = item.Product.Sku,
        //         CurrentStock = item.CurrentStock
        //     };

        //     response.EntireInventory.Add(i);
        // }

        // return Ok(response);
    }

    [HttpGet("{sku}")]
    public async Task<ActionResult<InventoryDTO>> GetBySku(string sku)
    {
        var item = await _service.BySkuAsync(sku);

        if (item is null)
        {
            return NotFound();
        }
        else
        {
            var mappedItem = _mapper.Map<InventoryDTO>(item);
            return Ok(mappedItem);
        }

        // var item = await _repo.GetInventoryItemBySkuAsync(sku);

        // if (item is null)
        //     return NotFound();
        // var response = new InventoryReturnDTO
        // {
        //     Name = item.Product.Name,
        //     Sku = item.Product.Sku,
        //     CurrentStock = item.CurrentStock
        // };


        // return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<InventoryDTO>> Create(InventoryCreateDto newInv)
    {
        var created = await _service.AddAsync(newInv);
        var response = _mapper.Map<InventoryDTO>(created);

        return CreatedAtAction(nameof(GetBySku), new { sku = response.Sku }, response);
    }

    [HttpDelete("{sku}")]
    public async Task<ActionResult> Delete(string sku)
    {
        bool isDeleted = await _service.RemoveAsync(sku);

        if (isDeleted)
        {
            return NoContent();
        }
        else
        {
            return NotFound();
        }
    }
}