using System;
using TicketServiceLib.DTOs;
using TicketServiceLib.Models;

namespace TicketServiceLib.Interfaces;

public interface ITicket
{
    /// <summary>
    /// Asynchronously creates a ticket
    /// </summary>
    /// <param name="ticket"></param>
    /// <returns></returns>
    Task<Ticket> CreateTicketAsync(CreateTicketDTO ticket);
    /// <summary>
    /// Asynchronously returns a ticket
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    Task<Ticket> GetTicketByIdAsync(int Id);
    /// <summary>
    /// Asynchronously tickets in a list
    /// </summary>
    /// <returns></returns>
    Task<List<Ticket>> GetAllTicketAsync();
    /// <summary>
    /// Updates and existing ticket
    /// </summary>
    /// <param name="ticket"></param>
    /// <returns></returns>
    Task UpdateTicket(Ticket ticket);

    /// <summary>
    /// Deletes existing ticket
    /// </summary>
    /// <param name="ticket"></param>
    /// <returns></returns>
    Task DeleteTicket(Ticket ticket);
  
}
