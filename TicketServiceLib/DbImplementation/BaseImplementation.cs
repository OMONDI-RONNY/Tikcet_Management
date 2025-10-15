using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TicketServiceLib.Data;
using TicketServiceLib.DTOs;
using TicketServiceLib.Interfaces;
using TicketServiceLib.Models;

namespace TicketServiceLib.DbImplementation;

public class BaseImplementation : ITicket, IFile
{
    private readonly AppDbContext _context;
    public BaseImplementation(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Ticket> CreateTicketAsync(CreateTicketDTO ticket)
    {
        try
        {
            Ticket createTicket = new Ticket()
            {
                Title = ticket.Title,
                Description = ticket.Description,
                Assignee = ticket.Assignee,
                TicketStatus = ticket.TicketStatus,
                PromiseDate = ticket.PromiseDate
            };

            if (ticket.AttachedFiles != null && ticket.AttachedFiles.Any())
            {
                foreach (var file in ticket.AttachedFiles)
                {
                    var attacheFile = new TicketAttachMent { FileName = file.FileName };
                    createTicket.Attachments!.Add(attacheFile);
                    Console.WriteLine(ticket.AttachedFiles.Count);

                }
            }
            await _context.Tickets.AddAsync(createTicket);
            await _context.SaveChangesAsync();

            return createTicket;
        }
        catch (System.Exception)
        {

            throw;
        }
    }

    public async Task DeleteTicket(Ticket ticket)
    {
        if (ticket == null)
            throw new NullReferenceException("Ticket is null");
        try
        {
            _context.Tickets.Remove(ticket);
            var files = _context.Files.Where(id => id.TicketId == ticket.TicketId).Select(f => f.FileName).ToList();
            if (files != null)
            {
                foreach (var file in files)
                {
                    DeleteFile(file);
                }
            }
            await _context.SaveChangesAsync();

        }
        catch (System.Exception)
        {

            throw;
        }
    }

    public async Task<List<Ticket>> GetAllTicketAsync()
    {
        try
        {
            var ticketList = await _context.Tickets.Include(file => file.Attachments).ToListAsync();
            if (ticketList == null)
            {
                throw new NullReferenceException("Does not exist");
            }

            return ticketList;

        }
        catch (System.Exception)
        {

            throw;
        }
    }

    public async Task<Ticket> GetTicketByIdAsync(int Id)
    {
        // if (Id < 0)
        // throw new ArgumentException("User ID must be greator than 0");
        try
        {
            var ticket = await _context.Tickets.Include(f => f.Attachments).FirstOrDefaultAsync(id => id.TicketId == Id);

            // if (ticket == null)
            // {
            //     throw new NullReferenceException("Ticket not found");
            // }

            return ticket!;
        }
        catch (System.Exception)
        {

            throw;
        }
    }

    public async Task UpdateTicket(Ticket ticket)
    {
        try
        {
            if (ticket == null)
            {
                throw new NullReferenceException();
            }

            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
        }
        catch (System.Exception)
        {

            throw;
        }
    }

    public async Task SaveFileAsync(IFormFile file)
    {
        using (var stream = System.IO.File.Create(GetFilePath(file.FileName)))
        {
            await file.CopyToAsync(stream);
        }
    }
    public void DeleteFile(string fileName)
    {
        if (System.IO.File.Exists(GetFilePath(fileName)))
        {
            System.IO.File.Delete(GetFilePath(fileName));
        }
    }

    public string GetFilePath(string fileName) => "uploads/" + fileName;


}
