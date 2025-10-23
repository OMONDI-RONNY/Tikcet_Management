using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NSCore.DatabaseContext;
using NSCore.DatabaseProviders;
using TicketServiceLib.Data;
using TicketServiceLib.DTOs;
using TicketServiceLib.Interfaces;
using TicketServiceLib.Models;
using TicketServiceLib.DbImplementation;
using Microsoft.AspNetCore.Http;
using TicketServiceLib.Enumerations;

namespace TicketServiceLib;

public class ManageTickets : BackgroundService, ITicket
{
    private ITicket _connection;
    private bool _isContextCreated;
    private bool _applyMigrationsAutomatically;
    private INsContextInit _initializer;
    public async Task<Ticket> CreateTicketAsync(CreateTicketDTO ticket)
    {
        EnsureContext();
        var createTicket = await _connection.CreateTicketAsync(ticket);
        return createTicket;
    }

    public Task DeleteTicket(int ticketId)
    {
        EnsureContext();
        return _connection.DeleteTicket(ticketId);
    }

    public async Task<List<Ticket>> GetAllTicketsAsync(int pageNumber,int pageSize)
    {
        EnsureContext();
        return await _connection.GetAllTicketsAsync(pageNumber,pageSize);
    }

    public async Task<Ticket> GetTicketByIdAsync(int Id)
    {
        EnsureContext();
        return await _connection.GetTicketByIdAsync(Id);
    }

    public async Task UpdateTicketAsync(int ticketId,UpdateTicketDTO ticket)
    {
        EnsureContext();
        await _connection.UpdateTicketAsync(ticketId,ticket);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        if (_connection == null)
        {
            throw new InvalidOperationException("Database connection is not initialized.");
        }

        if (_applyMigrationsAutomatically)
        {
            await _initializer.ApplyMigrationsAsync(stoppingToken);
        }

        if (_initializer.IsContextCreated())
        {
            _isContextCreated = true;
        }
    }
    public ManageTickets(IDatabaseConfig config, IDbContextFactory<AppDbContext> contextFactory, bool applyMigrationsAutomatically)
    {
        _applyMigrationsAutomatically = applyMigrationsAutomatically;
        _connection = config switch
        {
            PSQLDb => new PostgreSQL(contextFactory),
            _ => throw new ArgumentException("Unsupported database type.")


        };
        if (_connection == null)
            throw new InvalidOperationException("Failed to initialize a valid INsContextInitializer.");
        _initializer = _connection as INsContextInit ?? throw new InvalidOperationException("Failed to initialize a valid INsContextInitializer.");
    }

    private void EnsureContext()
    {
        if (!_isContextCreated)
        {
            throw new InvalidOperationException("Failed to initialize the database context.");
        }
    }

    public async Task SaveFileAsync(IFormFile file)
    {
        await _connection.SaveFileAsync(file);
    }

    public void DeleteFile(string fileName)
    {
        _connection.DeleteFile(fileName);
    }

    public async Task<IEnumerable<Ticket>> GetTicketsByStatusAsync(TicketStatus status)
    {
        return await _connection.GetTicketsByStatusAsync(status);
    }
}
