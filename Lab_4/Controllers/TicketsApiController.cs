using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab_4.Models;

namespace Lab_4.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TicketsApiController : ControllerBase
{
    private readonly AppDbContext _context;

    public TicketsApiController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/TicketsApi
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Ticket>>> GetTickets()
    {
        return await _context.Tickets
            .Include(t => t.Passenger)
            .Include(t => t.CarriageType)
            .Include(t => t.Train)
                .ThenInclude(tr => tr!.Destination)
            .ToListAsync();
    }

    // GET: api/TicketsApi/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Ticket>> GetTicket(int id)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Passenger)
            .Include(t => t.CarriageType)
            .Include(t => t.Train)
                .ThenInclude(tr => tr!.Destination)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (ticket == null) return NotFound();
        return ticket;
    }

    // POST: api/TicketsApi
    [HttpPost]
    public async Task<ActionResult<Ticket>> CreateTicket(Ticket ticket)
    {
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
    }

    // PUT: api/TicketsApi/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTicket(int id, Ticket ticket)
    {
        if (id != ticket.Id) return BadRequest();

        _context.Entry(ticket).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Tickets.Any(e => e.Id == id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/TicketsApi/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTicket(int id)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null) return NotFound();

        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}