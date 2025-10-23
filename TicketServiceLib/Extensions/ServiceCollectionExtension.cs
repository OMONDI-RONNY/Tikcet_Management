using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using NSCore.DatabaseContext;
using NSCore.DbContextHelper;
using NSCore.Models;
using TicketServiceLib.Data;
using TicketServiceLib.Interfaces;

namespace TicketServiceLib.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddTicketManager(
            this IServiceCollection services,
            IDatabaseConfig inputModel,
            DbSetupOptions? dbOptions = null,
            bool applyMigrationsAutomatically = true
            )
           
        {
            services.AddCustomDbContextFactory<AppDbContext>(inputModel, dbOptions);
            services.AddSingleton<ITicket>(provider =>
            {
                var contextFactory = provider.GetRequiredService<IDbContextFactory<AppDbContext>>();

                
                return new ManageTickets(
                    inputModel, contextFactory, applyMigrationsAutomatically);
            });


            // Register as hosted service
            services.AddSingleton<IHostedService>(provider =>
            {
                var manageUsersService = provider.GetRequiredService<ITicket>();
                return (ManageTickets)manageUsersService;
            });

            return services;
        }
}
