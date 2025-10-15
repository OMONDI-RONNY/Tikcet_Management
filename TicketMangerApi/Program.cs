
using Microsoft.EntityFrameworkCore;
using TicketServiceLib.Data;
using TicketServiceLib.DbImplementation;
using TicketServiceLib.DTOs;
using TicketServiceLib.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AppDbContext>(option => option.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(typeof(TicketAutoMapper).Assembly);
builder.Services.AddControllers();
builder.Services.AddScoped<ITicket, BaseImplementation>();
builder.Services.AddScoped<IFile,BaseImplementation>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

