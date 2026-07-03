using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TicketSystem.Data;

public class Ticket
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer? customer { get; set; }

    public int EventId { get; set; }

    public Event? evento { get; set; }

    [Required]
    [Precision(10, 2)]
    public double Price { get; set; }

    [Required]
    public string SeatNumber { get; set; } = default!;

    [Required]
    public string Section { get; set; } = default!;

    public bool IsVIP { get; set; }
    public bool IsFinished { get; set; }

    public DateTime? finishedUtc { get; set; }
}