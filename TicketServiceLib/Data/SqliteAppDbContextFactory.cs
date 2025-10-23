using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using static TicketServiceLib.Data.ContextVariants;

namespace TicketServiceLib.Data;

public class SqliteAppDbContextFactory : IDesignTimeDbContextFactory<SqliteContext>
{
    public SqliteContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        var connectionString = "Data source=ticket_management.db";
        optionsBuilder.UseSqlite(connectionString);
        return new SqliteContext(optionsBuilder.Options);
    }
}
