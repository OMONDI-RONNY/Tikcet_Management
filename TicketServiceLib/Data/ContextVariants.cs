using System;
using Microsoft.EntityFrameworkCore;

namespace TicketServiceLib.Data;

public static class ContextVariants
{
    /// <summary>
    /// Postgres context
    /// </summary>
    public class PostgresContext : AppDbContext
    {
        /// <summary>
        /// Initializes a new instance of PostgresContext
        /// </summary>
        /// <param name="options"></param>
        public PostgresContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
    }
    /// <summary>
    /// Sql context
    /// </summary>
    public class SqlServerContext : AppDbContext
    {
        /// <summary>
        /// Initialize a new instance of sqlserver context
        /// </summary>
        /// <param name="options"></param>
        public SqlServerContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
    }
    /// <summary>
    /// Sqlite context
    /// </summary>
    public class SqliteContext : AppDbContext
    {
        /// <summary>
        /// Intialize a new instance of Sqlite context
        /// </summary>
        /// <param name="options"></param>
        public SqliteContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
    }
    /// <summary>
    /// MySql Server context
    /// </summary>
    public class MySQLContext:AppDbContext
    {
        /// <summary>
        /// Initialize a new instance of MySql Server
        /// </summary>
        /// <param name="options"></param>
        public MySQLContext(DbContextOptions<AppDbContext> options):base(options)
        {
            
        }
    }
}
