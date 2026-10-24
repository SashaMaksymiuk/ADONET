using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Lab_4.Models;

namespace Lab_4.Pages.Tickets;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context) => _context = context;

    public IList<Ticket> Tickets { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Tickets = await _context.Tickets
            .Include(t => t.Passenger)
            .Include(t => t.CarriageType)
            .Include(t => t.Train)
                .ThenInclude(tr => tr!.Destination)
            .OrderBy(t => t.Id)
            .ToListAsync();
    }
}