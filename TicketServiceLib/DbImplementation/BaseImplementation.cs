using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TicketServiceLib.Data;
using TicketServiceLib.DTOs;
using TicketServiceLib.Interfaces;
using TicketServiceLib.Models;
using Microsoft.AspNetCore.Hosting;
using NSCore.DatabaseContext;
using TicketServiceLib.Enumerations;
using System.Drawing;

namespace TicketServiceLib.DbImplementation;

public abstract class BaseImplementation : ITicket, INsContextInit
{
    protected readonly IDbContextFactory<AppDbContext> _context;
    protected BaseImplementation(IDbContextFactory<AppDbContext> contextFactory)
    {
        _context = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    #region 
    protected virtual AppDbContext CreateMigrationContext()
    {
        return _context.CreateDbContext();
    }

    public virtual async Task ApplyMigrationsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var context = CreateMigrationContext();
            var canConnect = await context.Database.CanConnectAsync();
            if (!canConnect)
            {
                throw new InvalidOperationException("Database doesn't exist. Please create and configure it first.");
            }

            var appliedMigrations = await context.Database.GetAppliedMigrationsAsync(cancellationToken);
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync(cancellationToken);

            if (pendingMigrations.Any())
            {
                await context.Database.MigrateAsync(cancellationToken);
            }
        }
        catch (Exception ex) when (IsTableAlreadyExistsError(ex))
        {
            await HandleMigrationConflictAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new Exception($"Migration failed: {ex.Message}", ex);
        }
    }
    protected virtual bool IsTableAlreadyExistsError(Exception ex)
    {
        return false;
    }
    protected virtual async Task MarkMigrationAsAppliedAsync(string migrationId, CancellationToken cancellationToken)
    {
        using var context = _context.CreateDbContext();
        var provider = context.Database.ProviderName;
        var productVersion = typeof(DbContext).Assembly.GetName().Version?.ToString() ?? "8.0.0";

        // This is a generic implementation - override in derived classes for database-specific SQL
        var rawQuery = GetMarkMigrationSql();
        var parameters = GetMarkMigrationParameters(migrationId, productVersion);

        await context.Database.ExecuteSqlRawAsync(rawQuery, parameters, cancellationToken);
    }

    protected abstract string GetMarkMigrationSql();
    protected abstract object[] GetMarkMigrationParameters(string migrationId, string productVersion);

    private async Task HandleMigrationConflictAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var context = _context.CreateDbContext();
            var allMigrations = context.Database.GetMigrations().ToList();
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync(cancellationToken);
            var latestMigration = allMigrations.LastOrDefault();

            if (latestMigration != null && pendingMigrations.Contains(latestMigration))
            {
                foreach (var migration in pendingMigrations)
                {
                    await MarkMigrationAsAppliedAsync(migration, cancellationToken);
                }
            }
            else
            {
                throw new InvalidOperationException("Could not resolve conflict: No valid latest migration found.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to resolve migration conflict: {ex.Message}", ex);
        }
    }
    #endregion
    public bool IsContextCreated()
    {
        try
        {
            using var context = _context.CreateDbContext();
            return context.Database.CanConnect();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to check if the context is created.", ex);
        }
    }
    public async Task<Ticket> CreateTicketAsync(CreateTicketDTO ticketDto)
    {
        if (ticketDto == null)
            throw new ArgumentNullException(nameof(ticketDto));

        var createTicket = new Ticket()
        {
            Title = ticketDto.Title,
            Description = ticketDto.Description,
            Assignee = ticketDto.Assignee,
            TicketStatus = ticketDto.TicketStatus,
            PromiseDate = ticketDto.PromiseDate
        };

        if (ticketDto.AttachedFiles != null && ticketDto.AttachedFiles.Any())
        {
            foreach (var file in ticketDto.AttachedFiles)
            {
                createTicket.Attachments.Add(new TicketAttachMent { FileName = file.FileName, TicketId = createTicket.TicketId });
                await SaveFileAsync(file);
            }
        }
        using var context = _context.CreateDbContext();
        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            await context.Tickets.AddAsync(createTicket);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return createTicket;

        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync();
            throw new InvalidOperationException("Failed to create ticket", ex);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new InvalidOperationException("An unexpected error occured while adding the ticket", ex);
        }

    }

    public async Task DeleteTicket(int ticketId)
    {
        if (ticketId < 0)
            throw new ArgumentException("Ticket Id must be greator than zero.", nameof(ticketId));

        using var context = _context.CreateDbContext();
        using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var ticket = await GetTicketByIdAsync(ticketId);
            context.Tickets.Remove(ticket);
            var files = context.Files.Where(id => id.TicketId == ticket.TicketId).Select(f => f.FileName).ToList();
            if (files != null)
            {
                foreach (var file in files)
                {
                    DeleteFile(file);
                }
            }
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync();
            throw new InvalidOperationException("Failed to remove the ticket", ex);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new InvalidOperationException("An error occured while deleting the ticket", ex);
        }

    }
    public async Task<List<Ticket>> GetAllTicketsAsync(int pageNumber, int pageSize)
    {
        using var context = _context.CreateDbContext();
        var ticketList = await context.Tickets
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .Include(file => file.Attachments)
                        .ToListAsync();//eager loading

        return ticketList;
    }

    public async Task<Ticket> GetTicketByIdAsync(int Id)
    {
        using var context = _context.CreateDbContext();
        var ticket = await context.Tickets.Include(f => f.Attachments).FirstOrDefaultAsync(id => id.TicketId == Id);//eager loading
        if (ticket == null)
            throw new ArgumentException("Ticket not found");
        return ticket;
    }

    public async Task UpdateTicketAsync(int ticketId, UpdateTicketDTO ticketDto)
    {
        if (ticketId < 0)
            throw new ArgumentException("Ticket Id must be greater than 0");

        if (ticketDto == null)
            throw new ArgumentNullException("Ticket cannot be null");

        using var context = _context.CreateDbContext();
        using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var existingTicket = await GetTicketByIdAsync(ticketId);
            if (existingTicket == null)
                throw new InvalidOperationException($"Ticket Id {ticketId} not found");

            if (!string.IsNullOrWhiteSpace(ticketDto.Title))
                existingTicket.Title = ticketDto.Title;

            if (!string.IsNullOrWhiteSpace(ticketDto.Description))
                existingTicket.Description = ticketDto.Description;

            if (!string.IsNullOrWhiteSpace(ticketDto.Assignee))
                existingTicket.Assignee = ticketDto.Assignee;

            if (existingTicket.TicketStatus != ticketDto.TicketStatus && ticketDto.TicketStatus != null)
                existingTicket.TicketStatus = (Enumerations.TicketStatus)ticketDto.TicketStatus;

            if (!existingTicket.PromiseDate.Equals(ticketDto.PromiseDate) && ticketDto.PromiseDate != null)
                existingTicket.PromiseDate = existingTicket.PromiseDate;

            if (ticketDto.AttachedFiles != null)
                foreach (var file in ticketDto.AttachedFiles)
                {
                    existingTicket.Attachments.Add(new TicketAttachMent { FileName = file.FileName, TicketId = existingTicket.TicketId });
                    await SaveFileAsync(file);
                }

            context.Tickets.Update(existingTicket);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync();
            throw new InvalidOperationException("Failed to update the ticket", ex);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new InvalidOperationException("An error ocurred while updating the ticket", ex);
        }
    }
    public async Task SaveFileAsync(IFormFile file)
    {
        string folderName = "TicketFiles";
        if (!Directory.Exists(folderName))
            Directory.CreateDirectory(folderName);

        string filePath = Path.Combine(folderName, file.FileName);
        using (var stream = System.IO.File.Create(filePath))
        {
            await file.CopyToAsync(stream);
        }
    }
    public void DeleteFile(string fileName)
    {
        string filePath = Path.Combine("TicketFiles", fileName);
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }
    }

    public async Task<IEnumerable<Ticket>> GetTicketsByStatusAsync(TicketStatus ticketStatus)
    {
        using var context = _context.CreateDbContext();
        return await context.Tickets.Where(status => status.TicketStatus == ticketStatus).ToListAsync();
    }
}
