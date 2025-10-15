using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketServiceLib.DTOs;
using TicketServiceLib.Interfaces;
using TicketServiceLib.Models;

namespace TicketServiceLib.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly ITicket _service;
        private readonly IFile _fileService;
        private readonly IMapper _mapper;
        public TicketsController(ITicket service, IMapper mapper, IFile fileService)
        {
            _service = service;
            _mapper = mapper;
            _fileService = fileService;
        }

        [HttpPost]
        public async Task<ActionResult> CreateTicket([FromForm] CreateTicketDTO ticket)
        {
            if (ticket == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var createTicket = await _service.CreateTicketAsync(ticket);
            return CreatedAtAction(nameof(GetById), new { Id = createTicket.TicketId }, createTicket);

        }

        [HttpGet("{Id:int}")]
        public async Task<ActionResult<Ticket>> GetById(int Id)
        {
            var ticket = await _service.GetTicketByIdAsync(Id);
            if (ticket == null) return NotFound("Ticket not found");

            return Ok(ticket);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetAll()
        {
            var ticketList = await _service.GetAllTicketAsync();
            if (ticketList == null) return BadRequest("Ticket list not existing");
            return ticketList;
        }

        [HttpDelete("{Id}")]
        public async Task<ActionResult> Delete(int Id)
        {
            if (Id < 0)
            {
                return BadRequest();
            }
            var ticket = await _service.GetTicketByIdAsync(Id);
            if (ticket == null) return NotFound("Ticket not found");
            await _service.DeleteTicket(ticket);
            return NoContent();
        }

        [HttpPut]
        public async Task<ActionResult> Update(int Id,[FromForm] UpdateTicketDTO ticket)
        {
            if (Id < 0 || ticket == null)
            {
                return BadRequest();
            }

            var existingTicket = await _service.GetTicketByIdAsync(Id);
            if (existingTicket == null) return NotFound();

            existingTicket.Title = ticket.Title ?? existingTicket.Title;
            existingTicket.Description = ticket.Description ?? existingTicket.Description;
            existingTicket.Assignee = ticket.Assignee ?? existingTicket.Assignee;
            if (existingTicket.TicketStatus != ticket.TicketStatus && ticket.TicketStatus != null)
            {
                existingTicket.TicketStatus = (Enumerations.TicketStatus)ticket.TicketStatus;
            }

            await _service.UpdateTicket(existingTicket);
            return NoContent();
        }
    }
}
