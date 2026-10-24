using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab_3.Models;

namespace Lab_3.Controllers;

public class TicketsController : Controller
{
    private readonly AppDbContext _context;

    public TicketsController(AppDbContext context)
    {
        _context = context;
    }

    // 1. Read (Огляд усіх записів)
    public async Task<IActionResult> Index()
    {
        var tickets = await _context.Tickets
            .Include(t => t.Passenger)
            .Include(t => t.CarriageType)
            .Include(t => t.Train)
                .ThenInclude(tr => tr!.Destination)
            .OrderBy(t => t.Id)
            .ToListAsync();

        return View(tickets);
    }

    // 2. Create (Форма створення)
    public IActionResult Create()
    {
        PopulateDropdowns();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Ticket ticket)
    {
        if (ModelState.IsValid)
        {
            _context.Add(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        PopulateDropdowns(ticket);
        return View(ticket);
    }

    // 3. Edit (Редагування)
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null) return NotFound();

        PopulateDropdowns(ticket);
        return View(ticket);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Ticket ticket)
    {
        if (id != ticket.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(ticket);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Tickets.Any(e => e.Id == ticket.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        PopulateDropdowns(ticket);
        return View(ticket);
    }

    // 4. Delete (Видалення)
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var ticket = await _context.Tickets
            .Include(t => t.Passenger)
            .Include(t => t.CarriageType)
            .Include(t => t.Train)
                .ThenInclude(tr => tr!.Destination)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (ticket == null) return NotFound();

        return View(ticket);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket != null)
        {
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private void PopulateDropdowns(Ticket? ticket = null)
    {
        ViewBag.PassengerId = new SelectList(_context.Passengers, "Id", "FullName", ticket?.PassengerId);
        ViewBag.TrainId = new SelectList(_context.Trains.Include(t => t.Destination)
            .Select(t => new { t.Id, Title = $"№{t.TrainNumber} ({t.TrainType}) -> {t.Destination!.Name}" }), "Id", "Title", ticket?.TrainId);
        ViewBag.CarriageTypeId = new SelectList(_context.CarriageTypes, "Id", "TypeName", ticket?.CarriageTypeId);
    }
}