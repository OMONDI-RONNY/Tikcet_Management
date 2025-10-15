using System;
using AutoMapper;
using TicketServiceLib.Models;

namespace TicketServiceLib.DTOs;

public class TicketAutoMapper : Profile
{
    public TicketAutoMapper()
    {
        CreateMap<Ticket, CreateTicketDTO>().ReverseMap();
        CreateMap<Ticket, UpdateTicketDTO>().ReverseMap();
    }
}
