using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketServiceLib.DTOs;
using TicketServiceLib.Enumerations;
using TicketServiceLib.Interfaces;
using TicketServiceLib.Models;

namespace TicketServiceLib.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly ITicket _service;
        public TicketsController(ITicket service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult> CreateTicket([FromForm] CreateTicketDTO ticket)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var createTicket = await _service.CreateTicketAsync(ticket);
                return CreatedAtAction(nameof(GetById), new { Id = createTicket.TicketId }, createTicket);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("{Id:int}")]
        public async Task<ActionResult<Ticket>> GetById(int Id)
        {
            try
            {
                var ticket = await _service.GetTicketByIdAsync(Id);
                return Ok(ticket);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetAll(int pageNumber = 1, int pageSize = 5)
        {
            var ticketList = await _service.GetAllTicketsAsync(pageNumber, pageSize);
            return ticketList;
        }
        [HttpGet("{status}")]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTickesByStatus(TicketStatus status)
        {
            var tickets = await _service.GetTicketsByStatusAsync(status);
            return tickets.ToList();
        }

        [HttpDelete("{Id}")]
        public async Task<ActionResult> Delete(int Id)
        {
            try
            {
                await _service.DeleteTicket(Id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Update(int ticketId, [FromForm] UpdateTicketDTO ticketDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            try
            {
                await _service.UpdateTicketAsync(ticketId, ticketDto);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
