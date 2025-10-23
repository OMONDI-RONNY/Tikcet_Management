using System;
using Microsoft.EntityFrameworkCore;
using TicketServiceLib.Data;
using static TicketServiceLib.Data.ContextVariants;
using Npgsql;

namespace TicketServiceLib.DbImplementation;

public class PostgreSQL : BaseImplementation
{
    public PostgreSQL(IDbContextFactory<AppDbContext> contextFactory) : base(contextFactory)
    {

    }
    protected override AppDbContext CreateMigrationContext()
    {
        using var baseContext = _context.CreateDbContext();
        var connectionString = baseContext.Database.GetConnectionString();

        // Create PostgreSQL-specific context for proper migration discovery
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new PostgresContext(optionsBuilder.Options);
    }
    protected override object[] GetMarkMigrationParameters(string migrationId, string productVersion)
    {
        return new[]
       {
            new NpgsqlParameter("@migrationId", migrationId),
            new NpgsqlParameter("@productVersion", productVersion)
        };
    }

    protected override string GetMarkMigrationSql()
    {
        return @"INSERT INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"") VALUES (@migrationId, @productVersion) ON CONFLICT (""MigrationId"") DO NOTHING;";

    }
    protected override bool IsTableAlreadyExistsError(Exception ex)
    {
        return ex is PostgresException pgEx && pgEx.SqlState == "42P07";
    }
}
