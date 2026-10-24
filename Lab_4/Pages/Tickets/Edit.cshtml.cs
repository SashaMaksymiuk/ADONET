using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab_4.Models;

namespace Lab_4.Pages.Tickets;

public class EditModel : PageModel
{
    private readonly AppDbContext _context;

    public EditModel(AppDbContext context) => _context = context;

    [BindProperty]
    public Ticket Ticket { get; set; } = default!;

    public SelectList PassengersList { get; set; } = default!;
    public SelectList TrainsList { get; set; } = default!;
    public SelectList CarriageTypesList { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null) return NotFound();

        Ticket = ticket;
        PopulateLists();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            PopulateLists();
            return Page();
        }

        _context.Attach(Ticket).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return RedirectToPage("./Index");
    }

    private void PopulateLists()
    {
        PassengersList = new SelectList(_context.Passengers, "Id", "FullName", Ticket?.PassengerId);
        TrainsList = new SelectList(_context.Trains.Include(t => t.Destination)
            .Select(t => new { t.Id, Title = $"№{t.TrainNumber} ({t.TrainType}) -> {t.Destination!.Name}" }), "Id", "Title", Ticket?.TrainId);
        CarriageTypesList = new SelectList(_context.CarriageTypes, "Id", "TypeName", Ticket?.CarriageTypeId);
    }
}