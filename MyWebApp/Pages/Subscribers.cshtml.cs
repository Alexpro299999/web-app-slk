using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Pages;

public class SubscribersModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public SubscribersModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Subscriber> Subscribers { get; set; } = default!;

    public async Task OnGetAsync()
    {
        if (_context.Subscribers != null)
        {
            Subscribers = await _context.Subscribers
                .Include(s => s.Tariff)
                .ToListAsync();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var sub = await _context.Subscribers.FindAsync(id);
        if (sub != null)
        {
            _context.Subscribers.Remove(sub);
            await _context.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}