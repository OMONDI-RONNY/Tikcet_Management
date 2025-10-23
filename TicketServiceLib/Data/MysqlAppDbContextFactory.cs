using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using static TicketServiceLib.Data.ContextVariants;
namespace TicketServiceLib.Data;

public class MysqlAppDbContextFactory : IDesignTimeDbContextFactory<MySQLContext>
{
    public MySQLContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        var connectionString = "server=localhost;username=root;password=12345";
        optionsBuilder.UseMySQL(connectionString);
        return new MySQLContext(optionsBuilder.Options);
    }
}
