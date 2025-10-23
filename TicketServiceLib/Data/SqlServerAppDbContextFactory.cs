using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using static TicketServiceLib.Data.ContextVariants;

namespace TicketServiceLib.Data;

public class SqlServerAppDbContextFactory : IDesignTimeDbContextFactory<SqlServerContext>
{
    public SqlServerContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        var connectionString = "server=localhost;database=ticket_management;username=root;password=root1234;";
        optionsBuilder.UseSqlServer(connectionString);
        return new SqlServerContext(optionsBuilder.Options);
    }
}
