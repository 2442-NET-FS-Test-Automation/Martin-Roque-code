using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameStore.Data.Entities;

[Table("Customers")]
public class Customer
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = default!;

    [Required]
    public string Email { get; set; } = default!;

    public List<Buying> buyings { get; set; } = new();
}