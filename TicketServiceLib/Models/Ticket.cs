using System;
using System.ComponentModel.DataAnnotations;
using TicketServiceLib.Enumerations;

namespace TicketServiceLib.Models;

public class Ticket
{
    [Key]
    public int TicketId { get; set; }
    [Required]
    public required string Title { get; set; }
    public string? Description { get; set; }
    [Required]
    public required string Assignee { get; set; }
    public TicketStatus TicketStatus { get; set; }
    [DataType(DataType.Date)]
    public DateTime PromiseDate { get; set; }
    public ICollection<TicketAttachMent>? Attachments { get; set; }
}
