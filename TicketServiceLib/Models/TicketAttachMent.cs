using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TicketServiceLib.Models;

public class TicketAttachMent
{
    [Key]
    public int AttachementId { get; set; }
    public required string FileName { get; set; }
    public int TicketId { get; set; }
    [JsonIgnore]
    public virtual Ticket? Ticket { get; set; }
}
