using System;
using Microsoft.EntityFrameworkCore;
using TicketServiceLib.Models;

namespace TicketServiceLib.Data;

public class AppDbContext : DbContext
{
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketAttachMent> Files { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }
}
