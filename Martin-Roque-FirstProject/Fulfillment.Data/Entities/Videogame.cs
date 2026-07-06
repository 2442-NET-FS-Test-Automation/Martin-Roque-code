using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace VideoGameStore.Data.Entities;

public class Videogame
{
    public int Id { get; set; }

    public string SpeIden { get; set; } = default!;

    [MaxLength(100)]
    public string Name { get; set; } = default!;

    public string? ESRB { get; set; }

    [Precision(10, 2)]
    public decimal Price { get; set; }

    public InventoryItem? Inventory { get; set; }
}