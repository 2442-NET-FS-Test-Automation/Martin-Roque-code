using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketSystem.Data;

[Table("Customers")]
public class Customer
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string FirstName { get; set; } = default!;

    [Required, MaxLength(50)]
    public string LastName { get; set; } = default!;

    [Required]
    public string PaymentMethod { get; set; } = default!;

    [Required, MaxLength(70)]
    public string Email { get; set; } = default!;

    [Required]
    public int TelephoneNumber { get; set; }

    public List<Ticket> Tickets { get; set; } = new();
}