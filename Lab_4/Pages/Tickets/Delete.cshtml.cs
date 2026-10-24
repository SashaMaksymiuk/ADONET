using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Lab_4.Models;

namespace Lab_4.Pages.Tickets;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _context;

    public DeleteModel(AppDbContext context) => _context = context;

    [BindProperty]
    public Ticket Ticket { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Passenger)
            .Include(t => t.Train)
                .ThenInclude(tr => tr!.Destination)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (ticket == null) return NotFound();
        Ticket = ticket;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket != null)
        {
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }
        return RedirectToPage("./Index");
    }
}