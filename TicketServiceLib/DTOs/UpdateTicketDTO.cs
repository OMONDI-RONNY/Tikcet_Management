using System;
using Microsoft.AspNetCore.Http;
using TicketServiceLib.Enumerations;

namespace TicketServiceLib.DTOs;

public class UpdateTicketDTO
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Assignee { get; set; }
    public TicketStatus? TicketStatus { get; set; }
    public DateTime? PromiseDate { get; set; }
    public List<IFormFile>? AttachedFiles { get; set; }
}
