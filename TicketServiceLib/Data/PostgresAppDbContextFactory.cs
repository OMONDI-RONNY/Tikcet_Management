using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using static TicketServiceLib.Data.ContextVariants;

namespace TicketServiceLib.Data;

public class PostgresAppDbContextFactory : IDesignTimeDbContextFactory<PostgresContext>
{
    public PostgresContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        string connectionString = "Host=localhost;username=postgres;password=syncfusion1234;port=5432;database=ticket_management";
        optionsBuilder.UseNpgsql(connectionString);

        return new PostgresContext(optionsBuilder.Options);
    }
}
