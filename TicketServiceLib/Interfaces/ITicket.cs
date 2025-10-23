using System;
using Microsoft.AspNetCore.Http;
using TicketServiceLib.DTOs;
using TicketServiceLib.Enumerations;
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
    Task<List<Ticket>> GetAllTicketsAsync(int pageNumber,int pageSize);

    /// <summary>
    /// Updates and existing ticket
    /// </summary>
    /// <param name="ticket"></param>
    /// <returns></returns>
    Task UpdateTicketAsync(int Id, UpdateTicketDTO ticket);

    /// <summary>
    /// Returns list of tickets by status
    /// </summary>
    /// <param name="ticketStatus"></param>
    /// <returns></returns>
    // Task UpdateTicketStatusAsync(int ticketId,TicketStatus ticketStatus);
    Task<IEnumerable<Ticket>> GetTicketsByStatusAsync(TicketStatus status);

    /// <summary>
    /// Deletes existing ticket
    /// </summary>
    /// <param name="ticket"></param>
    /// <returns></returns>
    Task DeleteTicket(int ticketId);

    /// <summary>
    /// Saves file in a directory
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    Task SaveFileAsync(IFormFile file);
    /// <summary>
    /// Deletes file from a directory
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    void DeleteFile(string fileName);
    /// <summary>
    /// returns the filePath
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    // string GetFilePath(string fileName);
}
