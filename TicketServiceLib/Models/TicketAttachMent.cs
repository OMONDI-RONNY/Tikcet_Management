using System;
using System.ComponentModel.DataAnnotations;

namespace TicketServiceLib.Models;

public class TicketAttachMent
{
    [Key]
    public int AttacheId { get; set; }
    public required string FileName { get; set; }
    public int TicketId { get; set; }
    public Ticket? Ticket { get; set; }
}
